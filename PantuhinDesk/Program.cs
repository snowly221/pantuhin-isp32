using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;
using ClosedXML.Excel;
using Npgsql;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PantuhinDesk;

internal static class Ui
{
    static readonly Color Ink=Color.FromArgb(238,241,248), Muted=Color.FromArgb(151,161,180), Surface=Color.FromArgb(28,35,51), Canvas=Color.FromArgb(17,23,36), Blue=Color.FromArgb(221,55,58), Line=Color.FromArgb(48,57,75);
    public static Color AccentColor=>Blue; public static Color SurfaceColor=>Surface; public static Color CanvasColor=>Canvas; public static Color MutedColor=>Muted;
    public static void Apply(Control form)
    {
        form.BackColor=Canvas;form.ForeColor=Ink;form.Font=new Font("Segoe UI",9F);if(form is Form window)window.AutoScaleMode=AutoScaleMode.Font;
        StyleControls(form.Controls);
    }
    public static Panel Divider(int width)=>new(){Width=width,Height=1,BackColor=Line,Margin=new Padding(0,8,0,8),Tag="divider"};
    public static void EnableDarkScrollBar(Control control){if(control.IsHandleCreated)SetWindowTheme(control.Handle,"DarkMode_Explorer",null);else control.HandleCreated+=(_,_)=>SetWindowTheme(control.Handle,"DarkMode_Explorer",null);}
    [DllImport("uxtheme.dll",CharSet=CharSet.Unicode)] static extern int SetWindowTheme(IntPtr hwnd,string appName,string? idList);
    static void StyleControls(Control.ControlCollection controls)
    {
        foreach(Control c in controls)
        {
            if(c is DataGridView grid)
            {
                grid.BackgroundColor=Surface;grid.BorderStyle=BorderStyle.None;grid.EnableHeadersVisualStyles=false;grid.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(36,44,62);grid.ColumnHeadersDefaultCellStyle.ForeColor=Color.FromArgb(193,201,216);grid.ColumnHeadersDefaultCellStyle.Font=new Font("Segoe UI Semibold",9F);grid.ColumnHeadersHeight=42;grid.DefaultCellStyle.BackColor=Surface;grid.DefaultCellStyle.ForeColor=Ink;grid.DefaultCellStyle.SelectionBackColor=Color.FromArgb(74,42,53);grid.DefaultCellStyle.SelectionForeColor=Color.White;grid.DefaultCellStyle.Padding=new Padding(7,3,7,3);grid.AlternatingRowsDefaultCellStyle.BackColor=Color.FromArgb(32,40,57);grid.RowTemplate.Height=40;grid.GridColor=Line;grid.RowHeadersVisible=false;
            }
            else if(c is Button b)
            {
                if(IsInSidebar(b))
                {
                    var active=(b.Tag as string)=="nav-primary";b.FlatStyle=FlatStyle.Flat;b.FlatAppearance.BorderSize=0;b.FlatAppearance.MouseOverBackColor=Color.FromArgb(39,47,64);b.FlatAppearance.MouseDownBackColor=Color.FromArgb(49,56,72);b.BackColor=active?Blue:Color.FromArgb(12,16,26);b.ForeColor=Color.FromArgb(235,239,246);b.Font=new Font("Segoe UI",9.5F,active?FontStyle.Bold:FontStyle.Regular);b.TextAlign=ContentAlignment.MiddleLeft;b.Padding=new Padding(12,4,8,4);b.Cursor=Cursors.Hand;b.Height=42;
                    continue;
                }
                var primary=new[]{"Войти","Сохранить","Создать задачу","Принять в работу","Получить заказ"}.Contains(b.Text,StringComparer.OrdinalIgnoreCase);
                var danger=b.Text.Contains("Удалить",StringComparison.OrdinalIgnoreCase);b.FlatStyle=FlatStyle.Flat;b.FlatAppearance.BorderSize=primary||danger?0:1;b.FlatAppearance.BorderColor=Line;b.BackColor=primary?Blue:danger?Color.FromArgb(190,55,65):Surface;b.ForeColor=primary||danger?Color.White:Ink;b.Font=new Font("Segoe UI Semibold",9F);b.Padding=new Padding(12,4,12,4);b.Cursor=Cursors.Hand;b.Height=Math.Max(34,b.Height);
                b.MouseEnter+=(_,_)=>{if(!primary&&!danger)b.BackColor=Color.FromArgb(43,51,68);};b.MouseLeave+=(_,_)=>{if(!primary&&!danger)b.BackColor=Surface;};
            }
            else if(c is TextBox t){t.Font=new Font("Segoe UI",10F);t.BackColor=Surface;t.ForeColor=Ink;t.BorderStyle=BorderStyle.FixedSingle;}
            else if(c is ComboBox cb){cb.Font=new Font("Segoe UI",9.5F);cb.FlatStyle=FlatStyle.Flat;cb.BackColor=Surface;cb.ForeColor=Ink;}
            else if(c is Label label){label.ForeColor=(label.Tag as string)=="brand"?Color.White:IsInSidebar(label)?Muted:(label.Tag as string)=="muted"?Muted:Ink;}
            else if(c is Panel or FlowLayoutPanel or TableLayoutPanel){if((c.Tag as string)!="sidebar"&&(c.Tag as string)!="canvas"&&(c.Tag as string)!="divider"&&(c.Tag as string)!="accent")c.BackColor=Surface;}
            if(c.HasChildren)StyleControls(c.Controls);
        }
    }
    static bool IsInSidebar(Control control){for(Control? p=control;p is not null;p=p.Parent)if(p.Tag as string=="sidebar")return true;return false;}
}

internal sealed class RoundedPanel : Panel
{
    public int Radius { get; set; }=12;
    public RoundedPanel(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
    protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;using var path=Path();using var brush=new SolidBrush(BackColor);e.Graphics.FillPath(brush,path);}
    protected override void OnResize(EventArgs e){base.OnResize(e);if(Width<2||Height<2)return;using var p=Path();Region=new Region(p);}
    GraphicsPath Path(){var p=new GraphicsPath();var r=new Rectangle(0,0,Math.Max(1,Width-1),Math.Max(1,Height-1));var d=Math.Min(Radius*2,Math.Min(r.Width,r.Height));p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
}

internal sealed class RoundedButton : Button
{
    public int Radius { get; set; }=9;
    public RoundedButton(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;}
    protected override void OnPaint(PaintEventArgs e){e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;var r=new Rectangle(0,0,Math.Max(1,Width-1),Math.Max(1,Height-1));using var path=new GraphicsPath();int d=Radius*2;path.AddArc(r.X,r.Y,d,d,180,90);path.AddArc(r.Right-d,r.Y,d,d,270,90);path.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);path.AddArc(r.X,r.Bottom-d,d,d,90,90);path.CloseFigure();using var fill=new SolidBrush(Enabled?BackColor:SystemColors.ControlDark);e.Graphics.FillPath(fill,path);if(FlatAppearance.BorderSize>0){using var pen=new Pen(FlatAppearance.BorderColor);e.Graphics.DrawPath(pen,path);}var inner=new Rectangle(Padding.Left,Padding.Top,Math.Max(1,Width-Padding.Horizontal),Math.Max(1,Height-Padding.Vertical));var flags=TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis|(TextAlign==ContentAlignment.MiddleLeft?TextFormatFlags.Left|TextFormatFlags.NoPadding:TextFormatFlags.HorizontalCenter);TextRenderer.DrawText(e.Graphics,Text,Font,inner,Enabled?ForeColor:SystemColors.GrayText,flags);}
}

internal sealed class RoundedInput : UserControl
{
    readonly TextBox input=new(){BorderStyle=BorderStyle.None,BackColor=Color.FromArgb(28,35,51),ForeColor=Color.FromArgb(238,241,248),Font=new Font("Segoe UI",10F)};
    public new string Text { get=>input.Text; set=>input.Text=value; }
    public string PlaceholderText { get=>input.PlaceholderText; set=>input.PlaceholderText=value; }
    public bool UseSystemPasswordChar { get=>input.UseSystemPasswordChar; set=>input.UseSystemPasswordChar=value; }
    public new event EventHandler? TextChanged { add=>input.TextChanged+=value; remove=>input.TextChanged-=value; }
    public RoundedInput(){Height=40;MinimumSize=new Size(120,38);BackColor=Color.FromArgb(28,35,51);Padding=new Padding(12,0,10,0);Controls.Add(input);Resize+=(_,_)=>{input.Bounds=new Rectangle(Padding.Left,(Height-input.PreferredHeight)/2,Math.Max(10,Width-Padding.Horizontal),input.PreferredHeight);if(Width>2&&Height>2){using var p=Shape();Region=new Region(p);}};}
    protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;using var path=Shape();using var pen=new Pen(Color.FromArgb(51,60,78));e.Graphics.DrawPath(pen,path);}
    GraphicsPath Shape(){var p=new GraphicsPath();var r=new Rectangle(0,0,Math.Max(1,Width-1),Math.Max(1,Height-1));int d=Math.Min(16,Math.Min(r.Width,r.Height));p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
}

internal sealed class DarkScrollBar : Control
{
    int maximum,visible=1,value;bool dragging,hovered;int dragOffset;
    public event Action<int>? ValueChanged;
    public DarkScrollBar(){Width=16;MinimumSize=new Size(16,40);BackColor=Color.FromArgb(28,35,51);SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw,true);}
    public void SetMetrics(int rows,int visibleRows,int first){maximum=Math.Max(0,rows-Math.Max(1,visibleRows));visible=Math.Max(1,visibleRows);value=Math.Clamp(first,0,maximum);Visible=maximum>0;Invalidate();}
    protected override void OnPaint(PaintEventArgs e){e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;e.Graphics.Clear(BackColor);if(maximum<=0)return;int trackHeight=Math.Max(1,Height-8),minThumb=Math.Min(28,trackHeight),thumbHeight=Math.Clamp(trackHeight*visible/(maximum+visible),minThumb,trackHeight),range=Math.Max(1,trackHeight-thumbHeight),top=4+(int)Math.Round(range*(double)value/maximum);var r=new Rectangle(4,top,Math.Max(4,Width-8),thumbHeight);using var p=Round(r,Math.Min(thumbHeight,Math.Max(3,r.Width/2)));using var b=new SolidBrush(hovered||dragging?Color.FromArgb(142,151,170):Color.FromArgb(83,94,115));e.Graphics.FillPath(b,p);}
    protected override void OnMouseEnter(EventArgs e){base.OnMouseEnter(e);hovered=true;Invalidate();}
    protected override void OnMouseLeave(EventArgs e){base.OnMouseLeave(e);if(!dragging){hovered=false;Invalidate();}}
    protected override void OnMouseDown(MouseEventArgs e){base.OnMouseDown(e);if(maximum<=0)return;var thumb=Thumb();if(thumb.Contains(e.Location)){dragging=true;dragOffset=e.Y-thumb.Top;Capture=true;}else SetValue(value+(e.Y<thumb.Top?-visible:visible));}
    protected override void OnMouseMove(MouseEventArgs e){base.OnMouseMove(e);if(!dragging||maximum<=0)return;int trackHeight=Math.Max(1,Height-8),range=Math.Max(1,trackHeight-Thumb().Height);SetValue((int)Math.Round(Math.Clamp(e.Y-dragOffset-4,0,range)*(double)maximum/range));}
    protected override void OnMouseUp(MouseEventArgs e){base.OnMouseUp(e);dragging=false;Capture=false;Invalidate();}
    void SetValue(int next){next=Math.Clamp(next,0,maximum);if(next==value)return;value=next;Invalidate();ValueChanged?.Invoke(value);}
    Rectangle Thumb(){if(maximum<=0)return Rectangle.Empty;int track=Math.Max(1,Height-8),minThumb=Math.Min(28,track),h=Math.Clamp(track*visible/(maximum+visible),minThumb,track),range=Math.Max(1,track-h),top=4+(int)Math.Round(range*(double)value/maximum);return new Rectangle(4,top,Math.Max(4,Width-8),h);}
    static GraphicsPath Round(Rectangle r,int d){var p=new GraphicsPath();p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
}

internal sealed class DarkPicker : UserControl
{
    readonly RoundedButton button=new(){Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft,Padding=new Padding(10,3,8,3),BackColor=Color.FromArgb(28,35,51),ForeColor=Color.FromArgb(238,241,248)};
    readonly ContextMenuStrip menu=new(){ShowImageMargin=false,BackColor=Color.FromArgb(28,35,51),ForeColor=Color.FromArgb(238,241,248),Renderer=new DarkMenuRenderer()};int selected=-1;
    public List<object> Items { get; }=[];
    public event EventHandler? SelectedIndexChanged;
    public int SelectedIndex { get=>selected; set { if(value<0||value>=Items.Count){selected=-1;button.Text="Выбрать  ▾";return;}selected=value;button.Text=Convert.ToString(Items[value])+"  ▾";SelectedIndexChanged?.Invoke(this,EventArgs.Empty); } }
    public new string Text=>selected>=0&&selected<Items.Count?Convert.ToString(Items[selected])??"":"";
    public DarkPicker(){Height=36;MinimumSize=new Size(90,36);BackColor=Color.Transparent;Controls.Add(button);button.Click+=(_,_)=>ShowMenu();}
    void ShowMenu(){menu.Items.Clear();for(var i=0;i<Items.Count;i++){var index=i;var item=new ToolStripMenuItem(Convert.ToString(Items[i])){Tag=index,ForeColor=Color.FromArgb(238,241,248),BackColor=Color.FromArgb(28,35,51),Font=new Font("Segoe UI",9.5F),Padding=new Padding(10,7,18,7)};item.Click+=(_,_)=>SelectedIndex=index;menu.Items.Add(item);}menu.Show(button,new Point(0,button.Height));}
    sealed class DarkMenuRenderer:ToolStripProfessionalRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e){using var b=new SolidBrush(Color.FromArgb(28,35,51));e.Graphics.FillRectangle(b,e.AffectedBounds);}
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e){if(!e.Item.Selected)return;var r=new Rectangle(Point.Empty,e.Item.Size);r.Inflate(-3,-1);using var b=new SolidBrush(Color.FromArgb(221,55,58));e.Graphics.FillRectangle(b,r);}
    }
}

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        QuestPDF.Settings.License = LicenseType.Community;
        try
        {
            var settings = Settings.Load();
            while (true)
            {
                using var login = new LoginForm(settings.ConnectionString);
                if (login.ShowDialog() != DialogResult.OK || login.User is null) break;
                using var main = new MainForm(settings.ConnectionString, login.User);
                main.ShowDialog();
                if (!main.LoggedOut) break;
            }
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Pantuhin Desk", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}

internal sealed class Settings
{
    public string ConnectionString { get; private init; } = "";
    public static Settings Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path)) throw new InvalidOperationException("Не найден appsettings.json. Скопируйте appsettings.example.json и укажите параметры PostgreSQL.");
        using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(path));
        return new Settings { ConnectionString = doc.RootElement.GetProperty("ConnectionString").GetString() ?? "" };
    }
}

internal enum Role { User, Manager, Admin }
internal sealed record Session(long Id, string Login, string Name, Role Role);

internal static class Db
{
    public static NpgsqlConnection Open(string cs) { var c = new NpgsqlConnection(cs); c.Open(); return c; }
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16); var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, 210_000, HashAlgorithmName.SHA256, 32);
        return $"pbkdf2${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }
    public static bool Verify(string password, string stored)
    {
        if (!stored.StartsWith("pbkdf2$", StringComparison.Ordinal)) return password == stored; // legacy credentials from the supplied dump
        try { var p = stored.Split('$'); return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(p[2]), Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(p[1]), 210_000, HashAlgorithmName.SHA256, 32)); }
        catch { return false; }
    }
    public static long NextId(NpgsqlConnection c, string table, string key)
    {
        using var cmd = new NpgsqlCommand($"SELECT COALESCE(MAX({key}),0)+1 FROM {table}", c);
        // The caller holds a transaction; serialize allocation across application instances.
        using var lockCmd = new NpgsqlCommand("SELECT pg_advisory_xact_lock(817263)", c); lockCmd.ExecuteNonQuery();
        return (long)cmd.ExecuteScalar()!;
    }
    public static readonly string[] Statuses = ["Новая", "В работе", "Выполнена", "Получена", "Отменена"];
    public static readonly string[] Positions = ["Электрик", "Сантехник", "Плотник", "Маляр", "Сварщик", "Плиточник", "Разнорабочий", "Прораб"];
    public static string Decode(string s)
    {
        // Restore common UTF-8-as-Windows-1251 mojibake in the supplied backup when possible.
        try { var b = Encoding.GetEncoding(1251, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetBytes(s); var fixedText = Encoding.UTF8.GetString(b); return fixedText.Contains('�') ? s : fixedText; }
        catch { return s; }
    }
}

internal sealed class LoginForm : Form
{
    readonly string cs; readonly RoundedInput login = new(), password = new(); public Session? User { get; private set; }
    public LoginForm(string connectionString)
    {
        cs=connectionString;Text="Pantuhin Desk — вход";Width=480;Height=570;StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;
        var canvas=new Panel{Dock=DockStyle.Fill,BackColor=Ui.CanvasColor,Tag="canvas"};var card=new RoundedPanel{Size=new Size(390,470),BackColor=Ui.SurfaceColor,Radius=18};var brandMark=new RoundedPanel{Location=new Point(30,28),Size=new Size(48,48),Radius=14,BackColor=Ui.AccentColor,Tag="accent"};brandMark.Controls.Add(new Label{Text="P",Dock=DockStyle.Fill,ForeColor=Color.White,Font=new Font("Segoe UI",20,FontStyle.Bold),TextAlign=ContentAlignment.MiddleCenter,Tag="brand"});
        var heading=new Label{Text="С возвращением",AutoSize=true,Font=new Font("Segoe UI",22,FontStyle.Bold),Location=new Point(30,96)};var subtitle=new Label{Text="Войдите в Pantuhin Desk",AutoSize=true,ForeColor=Ui.MutedColor,Tag="muted",Location=new Point(32,134)};
        login.PlaceholderText="Логин";login.Location=new Point(30,194);login.Size=new Size(330,42);password.PlaceholderText="Пароль";password.UseSystemPasswordChar=true;password.Location=new Point(30,252);password.Size=new Size(330,42);
        var enter=new RoundedButton{Text="Войти",Width=330,Height=44,Location=new Point(30,326)};var reg=new RoundedButton{Text="Создать аккаунт",Width=330,Height=40,Location=new Point(30,382)};enter.Click+=(_,_)=>SignIn();reg.Click+=(_,_)=>Register();AcceptButton=enter;card.Controls.AddRange([brandMark,heading,subtitle,login,password,enter,reg]);canvas.Controls.Add(card);canvas.Resize+=(_,_)=>card.Location=new Point((canvas.ClientSize.Width-card.Width)/2,(canvas.ClientSize.Height-card.Height)/2);Controls.Add(canvas);Ui.Apply(this);
    }
    void SignIn()
    {
        if(string.IsNullOrWhiteSpace(login.Text)||string.IsNullOrEmpty(password.Text)){Err("Введите логин и пароль.");return;}
        try { using var c=Db.Open(cs); foreach(var x in new[]{("admin","admin_id","full_name",Role.Admin),("manager","manager_id","full_name",Role.Manager),("app_user","user_id","login",Role.User)}) {
            using var cmd=new NpgsqlCommand($"SELECT {x.Item2},login,password,{x.Item3} FROM {x.Item1} WHERE login=@l",c); cmd.Parameters.AddWithValue("l",login.Text.Trim()); using var r=cmd.ExecuteReader();
            if(r.Read()) { var stored=r.GetString(2); if(!Db.Verify(password.Text,stored)){Err("Неверный логин или пароль.");return;} var name=x.Item3=="login"?login.Text:r.IsDBNull(3)?login.Text:Db.Decode(r.GetString(3)); User=new Session(r.GetInt64(0),r.GetString(1),name,x.Item4); if(!stored.StartsWith("pbkdf2$")){r.Close(); using var up=new NpgsqlCommand($"UPDATE {x.Item1} SET password=@p WHERE {x.Item2}=@id",c);up.Parameters.AddWithValue("p",Db.Hash(password.Text));up.Parameters.AddWithValue("id",User.Id);up.ExecuteNonQuery();} DialogResult=DialogResult.OK; Close(); return; }
        }} catch(Exception ex){Err("Не удалось подключиться или выполнить вход: "+ex.Message);} Err("Неверный логин или пароль.");
    }
    void Register()
    {
        var l=login.Text.Trim(); if(l.Length<3||password.Text.Length<8){Err("Для регистрации укажите логин от 3 символов и пароль не короче 8 символов.");return;}
        try { using var c=Db.Open(cs); using var tx=c.BeginTransaction(); var id=Db.NextId(c,"app_user","user_id"); using var cmd=new NpgsqlCommand("INSERT INTO app_user(user_id,login,password) VALUES(@id,@l,@p)",c,tx);cmd.Parameters.AddWithValue("id",id);cmd.Parameters.AddWithValue("l",l);cmd.Parameters.AddWithValue("p",Db.Hash(password.Text));cmd.ExecuteNonQuery();tx.Commit(); MessageBox.Show("Учётная запись создана. Теперь войдите."); }
        catch(PostgresException ex) when(ex.SqlState=="23505"){Err("Такой логин уже существует.");} catch(Exception ex){Err("Регистрация не выполнена: "+ex.Message);}
    }
    static void Err(string s)=>MessageBox.Show(s,"Проверка данных",MessageBoxButtons.OK,MessageBoxIcon.Warning);
}

internal sealed class MainForm : Form
{
    public bool LoggedOut { get; private set; }
    DataTable? taskCache;
    readonly System.Windows.Forms.Timer searchDebounce=new(){Interval=250};
    readonly Panel contentHost=new(){Dock=DockStyle.Fill,BackColor=Color.FromArgb(17,23,36),Tag="canvas"};Panel? dashboardPage;ProfileForm? profilePage;System.Windows.Forms.Timer? pageAnimation;
    readonly Label totalValue=new(){Text="0"}, newValue=new(){Text="0"}, activeValue=new(){Text="0"}, doneValue=new(){Text="0"};
    readonly string cs; readonly Session me; readonly DataGridView grid=new(){Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
    readonly RoundedInput search=new(){Width=190,PlaceholderText="Поиск по описанию/логину"}; readonly DarkPicker status=new(){Width=170}; readonly DarkPicker sort=new(){Width=170};readonly DarkScrollBar verticalScroll=new(){Dock=DockStyle.Right};
    public MainForm(string connectionString,Session session)
    {
        cs=connectionString;me=session;Text=$"Pantuhin Desk — {session.Name} ({RoleName(session.Role)})";Width=1280;Height=780;MinimumSize=new Size(1000,650);StartPosition=FormStartPosition.CenterScreen;
        status.Items.AddRange(["Все статусы", ..Db.Statuses]);status.SelectedIndex=0;sort.Items.AddRange(["Дата ↓", "Дата ↑", "Статус", "Пользователь"]);sort.SelectedIndex=0;search.Width=280;status.Width=170;sort.Width=170;
        var shell=new Panel{Dock=DockStyle.Fill,BackColor=Ui.CanvasColor,Tag="canvas"};var sidebar=new Panel{Dock=DockStyle.Left,Width=224,BackColor=Color.FromArgb(12,16,26),Tag="sidebar",Padding=new Padding(12)};var nav=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,BackColor=sidebar.BackColor,Padding=new Padding(2,12,2,12),Tag="sidebar"};
        var brand=new Label{Text="Pantuhin Desk",AutoSize=false,Width=188,Height=42,Font=new Font("Segoe UI",15,FontStyle.Bold),ForeColor=Color.White,TextAlign=ContentAlignment.MiddleLeft,Tag="brand"};nav.Controls.Add(brand);nav.Controls.Add(NavCaption(RoleName(me.Role)+"  ·  "+me.Name));nav.Controls.Add(NavCaption("РАБОТА"));nav.Controls.Add(NavButton("Заявки",ShowDashboard));
        var create=NavButton("＋   Новая заявка",AddTask,true);var edit=NavButton("Изменить заявку",EditTask);var delete=NavButton("Удалить заявку",DeleteTask);var accept=NavButton("Принять в работу",AcceptTask);var finish=NavButton("Завершить заявку",FinishTask);var receive=NavButton("Подтвердить получение",ReceiveTask);var worker=NavButton("Работники",Workers);var accounts=NavButton("Пользователи и роли",Accounts);create.Visible=me.Role is Role.User or Role.Admin;edit.Visible=delete.Visible=accounts.Visible=me.Role==Role.Admin;accept.Visible=finish.Visible=me.Role==Role.Manager;receive.Visible=me.Role==Role.User;nav.Controls.AddRange([create,edit,delete,accept,finish,receive,worker]);nav.Controls.Add(NavCaption("ОТЧЁТЫ"));nav.Controls.Add(NavButton("Экспорт в Excel",ExportExcel));nav.Controls.Add(NavButton("Экспорт в PDF",ExportPdf));nav.Controls.Add(NavButton("Обновить список",LoadTasks));nav.Controls.Add(NavCaption("АККАУНТ"));nav.Controls.Add(NavButton("Личный кабинет",Profile));nav.Controls.Add(NavButton("Выйти из аккаунта",Logout));sidebar.Controls.Add(nav);
        var content=new Panel{Dock=DockStyle.Fill,BackColor=Ui.CanvasColor,Padding=new Padding(28,22,28,24),Tag="canvas"};dashboardPage=content;var tableCard=new RoundedPanel{Dock=DockStyle.Fill,BackColor=Ui.SurfaceColor,Padding=new Padding(1),Radius=12};grid.ScrollBars=ScrollBars.None;var header=new Panel{Dock=DockStyle.Top,Height=86,BackColor=Ui.CanvasColor,Tag="canvas"};var heading=new Label{Text="Заявки",AutoSize=true,Font=new Font("Segoe UI",22,FontStyle.Bold),Location=new Point(0,0)};var subtitle=new Label{Text="Управление и контроль выполнения",AutoSize=true,ForeColor=Ui.MutedColor,Tag="muted",Location=new Point(2,43)};var identity=new Label{Text=$"{me.Name}   ·   {RoleName(me.Role)}",AutoSize=true,ForeColor=Ui.MutedColor,Tag="muted",Anchor=AnchorStyles.Top|AnchorStyles.Right,Location=new Point(800,16)};header.Controls.AddRange([heading,subtitle,identity]);header.Resize+=(_,_)=>identity.Left=header.ClientSize.Width-identity.Width;
        var filters=new FlowLayoutPanel{Dock=DockStyle.Top,Height=58,WrapContents=false,Padding=new Padding(0,8,0,8),BackColor=Ui.CanvasColor,Tag="canvas"};filters.Controls.AddRange([search,status,sort]);var reload=NavButton("Обновить",LoadTasks);reload.Width=112;reload.Height=36;reload.Tag=null;filters.Controls.Add(reload);
        var stats=new FlowLayoutPanel{Dock=DockStyle.Top,Height=94,WrapContents=false,Padding=new Padding(0,4,0,12),BackColor=Ui.CanvasColor,Tag="canvas"};stats.Controls.AddRange([MetricCard("Всего заявок",totalValue),MetricCard("Новые",newValue),MetricCard("В работе",activeValue),MetricCard("Завершено",doneValue)]);
        tableCard.Controls.Add(grid);tableCard.Controls.Add(verticalScroll);content.Controls.Add(tableCard);content.Controls.Add(stats);content.Controls.Add(filters);content.Controls.Add(header);contentHost.Controls.Add(content);shell.Controls.Add(contentHost);shell.Controls.Add(sidebar);Controls.Add(shell);
        verticalScroll.ValueChanged+=value=>{if(grid.RowCount==0)return;try{grid.FirstDisplayedScrollingRowIndex=Math.Clamp(value,0,grid.RowCount-1);}catch(InvalidOperationException){}};grid.Scroll+=(_,_)=>SyncVerticalScroll();grid.Resize+=(_,_)=>SyncVerticalScroll();grid.DataBindingComplete+=(_,_)=>SyncVerticalScroll();searchDebounce.Tick+=(_,_)=>{searchDebounce.Stop();ApplyTaskView();};status.SelectedIndexChanged+=(_,_)=>ApplyTaskView();sort.SelectedIndexChanged+=(_,_)=>ApplyTaskView();search.TextChanged+=(_,_)=>{searchDebounce.Stop();searchDebounce.Start();};grid.CellDoubleClick+=(_,_)=>EditTask();FormClosed+=(_,_)=>{searchDebounce.Dispose();pageAnimation?.Stop();pageAnimation?.Dispose();};Ui.Apply(this);LoadTasks();
    }
    Button B(string text,Action action){var b=new RoundedButton{Text=text,AutoSize=true,Height=34};b.Click+=(_,_)=>action();return b;}
    Button NavButton(string text,Action action,bool primary=false){var b=new RoundedButton{Text=text,Width=188,Height=42,Tag=primary?"nav-primary":"sidebar"};b.Click+=(_,_)=>action();return b;}
    Panel MetricCard(string title,Label value){var card=new RoundedPanel{Width=190,Height=70,Margin=new Padding(0,0,12,0),BackColor=Ui.SurfaceColor,Radius=11};var stripe=new Panel{Dock=DockStyle.Top,Height=3,BackColor=Ui.AccentColor,Tag="accent"};var caption=new Label{Text=title,AutoSize=true,ForeColor=Ui.MutedColor,Tag="muted",Location=new Point(14,10)};value.Font=new Font("Segoe UI",21,FontStyle.Bold);value.ForeColor=Color.White;value.Location=new Point(13,28);value.AutoSize=true;card.Controls.Add(value);card.Controls.Add(caption);card.Controls.Add(stripe);return card;}
    Label NavCaption(string text)=>new(){Text=text,AutoSize=false,Width=188,Height=30,Font=new Font("Segoe UI",8.5F,FontStyle.Bold),ForeColor=Color.FromArgb(155,167,186),TextAlign=ContentAlignment.MiddleLeft,Padding=new Padding(9,7,0,0),Tag="sidebar"};
    void Logout(){if(MessageBox.Show("Выйти из текущего аккаунта?","Выход",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;LoggedOut=true;Close();}
    void Profile(){profilePage??=new ProfileForm(cs,me);NavigateTo(profilePage);}
    void ShowDashboard(){if(dashboardPage is not null)NavigateTo(dashboardPage);}
    void NavigateTo(Control page){if(pageAnimation is not null){pageAnimation.Stop();pageAnimation.Dispose();pageAnimation=null;}var current=contentHost.Controls.Cast<Control>().FirstOrDefault(x=>x.Visible&&x!=page);if(current is not null)current.Visible=false;if(!contentHost.Controls.Contains(page))contentHost.Controls.Add(page);var width=contentHost.ClientSize.Width;page.Dock=DockStyle.None;page.Bounds=new Rectangle(width,0,width,contentHost.ClientSize.Height);page.Visible=true;page.BringToFront();var frame=0;var timer=new System.Windows.Forms.Timer{Interval=15};pageAnimation=timer;timer.Tick+=(_,_)=>{frame++;var t=Math.Min(1,frame/14.0);var eased=1-Math.Pow(1-t,3);page.Left=(int)Math.Round(width*(1-eased));if(t>=1){timer.Stop();page.Dock=DockStyle.Fill;timer.Dispose();if(ReferenceEquals(pageAnimation,timer))pageAnimation=null;}};timer.Start();}
    static string RoleName(Role r)=>r switch{Role.Admin=>"Администратор",Role.Manager=>"Менеджер",_=>"Пользователь"};
    void LoadTasks()
    {
        try { using var c=Db.Open(cs);const string sql="SELECT t.task_id,t.status,t.description,t.created_at,t.user_id,u.login AS user_login,t.manager_id,m.login AS manager_login FROM task t JOIN app_user u ON u.user_id=t.user_id JOIN manager m ON m.manager_id=t.manager_id";using var cmd=new NpgsqlCommand(sql+(me.Role==Role.User?" WHERE t.user_id=@uid":""),c);if(me.Role==Role.User)cmd.Parameters.AddWithValue("uid",me.Id);var dt=new DataTable();dt.Load(cmd.ExecuteReader());foreach(DataColumn column in dt.Columns)column.ReadOnly=false;foreach(DataRow row in dt.Rows){row["status"]=Db.Decode(row["status"].ToString()!);if(row["description"]!=DBNull.Value)row["description"]=Db.Decode(row["description"].ToString()!);row["user_login"]=Db.Decode(row["user_login"].ToString()!);row["manager_login"]=Db.Decode(row["manager_login"].ToString()!);}taskCache=dt;grid.DataSource=taskCache.DefaultView;grid.Columns["task_id"].HeaderText="ID";grid.Columns["status"].HeaderText="Статус";grid.Columns["description"].HeaderText="Описание";grid.Columns["created_at"].HeaderText="Создана";grid.Columns["user_id"].HeaderText="ID пользователя";grid.Columns["user_login"].HeaderText="Пользователь";grid.Columns["manager_id"].HeaderText="ID менеджера";grid.Columns["manager_login"].HeaderText="Менеджер";ApplyTaskView();
        }catch(Exception ex){MessageBox.Show("Не удалось загрузить задачи: "+ex.Message,"Ошибка");}
    }
    void ApplyTaskView(){if(taskCache is null)return;var all=taskCache.Rows.Cast<DataRow>().ToList();totalValue.Text=all.Count.ToString();newValue.Text=all.Count(r=>r["status"].ToString()==Db.Statuses[0]).ToString();activeValue.Text=all.Count(r=>r["status"].ToString()==Db.Statuses[1]).ToString();doneValue.Text=all.Count(r=>r["status"].ToString()==Db.Statuses[2]||r["status"].ToString()==Db.Statuses[3]).ToString();var view=taskCache.DefaultView;var filters=new List<string>();if(status.SelectedIndex>0)filters.Add("[status] = '"+status.Text.Replace("'","''")+"'");var term=search.Text.Trim().Replace("'","''").Replace("[","[[]").Replace("]","[]]").Replace("%","[%]").Replace("*","[*]");if(term.Length>0)filters.Add("([description] LIKE '%"+term+"%' OR [user_login] LIKE '%"+term+"%' OR [manager_login] LIKE '%"+term+"%' OR [status] LIKE '%"+term+"%')");view.RowFilter=string.Join(" AND ",filters);view.Sort=sort.SelectedIndex switch{1=>"created_at ASC, task_id ASC",2=>"status ASC, created_at DESC",3=>"user_login ASC, created_at DESC",_=>"created_at DESC, task_id DESC"};}
    void SyncVerticalScroll(){if(!grid.IsHandleCreated||grid.RowCount==0){verticalScroll.SetMetrics(0,1,0);return;}var visible=Math.Max(1,grid.DisplayedRowCount(false));int first;try{first=Math.Max(0,grid.FirstDisplayedScrollingRowIndex);}catch(InvalidOperationException){first=0;}verticalScroll.SetMetrics(grid.RowCount,visible,first);}
    long SelectedId()=>grid.CurrentRow is null?0:Convert.ToInt64(grid.CurrentRow.Cells["task_id"].Value);
    void AddTask(){using var f=new TaskDialog(cs,me,null);if(f.ShowDialog()==DialogResult.OK)LoadTasks();}
    void EditTask(){if(me.Role!=Role.Admin||SelectedId()==0)return;using var f=new TaskDialog(cs,me,SelectedId());if(f.ShowDialog()==DialogResult.OK)LoadTasks();}
    void DeleteTask(){var id=SelectedId();if(me.Role!=Role.Admin||id==0)return;if(MessageBox.Show("Удалить выбранную задачу?","Подтверждение",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;try{using var c=Db.Open(cs);using var cmd=new NpgsqlCommand("DELETE FROM task WHERE task_id=@id",c);cmd.Parameters.AddWithValue("id",id);cmd.ExecuteNonQuery();LoadTasks();}catch(Exception ex){MessageBox.Show("Не удалось удалить задачу: "+ex.Message);}}
    void SetStatus(string target,string[] allowed){var id=SelectedId();if(id==0)return;try{using var c=Db.Open(cs);using var tx=c.BeginTransaction();using var read=new NpgsqlCommand("SELECT status FROM task WHERE task_id=@id"+(me.Role==Role.User?" AND user_id=@uid":"")+" FOR UPDATE",c,tx);read.Parameters.AddWithValue("id",id);if(me.Role==Role.User)read.Parameters.AddWithValue("uid",me.Id);var raw=read.ExecuteScalar() as string;if(raw is null||!allowed.Contains(Db.Decode(raw))){MessageBox.Show("Статус уже изменён или действие недоступно.");return;}using var cmd=new NpgsqlCommand("UPDATE task SET status=@s WHERE task_id=@id AND status=@old",c,tx);cmd.Parameters.AddWithValue("s",target);cmd.Parameters.AddWithValue("id",id);cmd.Parameters.AddWithValue("old",raw);cmd.ExecuteNonQuery();tx.Commit();LoadTasks();}catch(Exception ex){MessageBox.Show("Не удалось изменить статус: "+ex.Message);}}
    void AcceptTask()=>SetStatus("В работе",["Новая"]); void FinishTask()=>SetStatus("Выполнена",["В работе"]); void ReceiveTask()=>SetStatus("Получена",["Выполнена"]);
    void Workers()=>new WorkerForm(cs,me).ShowDialog(this); void Accounts()=>new AccountForm(cs).ShowDialog(this);
    List<string[]> Rows(){var rows=new List<string[]>();foreach(DataGridViewRow r in grid.Rows)rows.Add(r.Cells.Cast<DataGridViewCell>().Select(x=>Convert.ToString(x.Value)??"").ToArray());return rows;}
    string[] Headers()=>grid.Columns.Cast<DataGridViewColumn>().Select(c=>c.HeaderText).ToArray();
    void ExportExcel(){using var s=new SaveFileDialog{Filter="Excel workbook (*.xlsx)|*.xlsx",FileName="Задачи.xlsx"};if(s.ShowDialog()!=DialogResult.OK)return;try{using var wb=new XLWorkbook();var ws=wb.Worksheets.Add("Задачи");for(int i=0;i<Headers().Length;i++)ws.Cell(1,i+1).Value=Headers()[i];var rr=Rows();for(int r=0;r<rr.Count;r++)for(int col=0;col<rr[r].Length;col++)ws.Cell(r+2,col+1).Value=rr[r][col];ws.Row(1).Style.Font.Bold=true;ws.Columns().AdjustToContents();wb.SaveAs(s.FileName);MessageBox.Show("Отчёт Excel сохранён.");}catch(Exception ex){MessageBox.Show("Не удалось создать Excel: "+ex.Message);}}
    void ExportPdf(){using var s=new SaveFileDialog{Filter="PDF (*.pdf)|*.pdf",FileName="Задачи.pdf"};if(s.ShowDialog()!=DialogResult.OK)return;try{var heads=Headers();var data=Rows();Document.Create(container=>container.Page(page=>{page.Margin(24);page.Header().Text("Отчёт по задачам").FontSize(18).Bold();page.Content().PaddingVertical(12).Table(t=>{t.ColumnsDefinition(c=>{foreach(var _ in heads)c.RelativeColumn();});t.Header(h=>{foreach(var x in heads)h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(x).FontSize(8).Bold();});foreach(var row in data)foreach(var cell in row)t.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(cell).FontSize(7);});page.Footer().AlignRight().Text($"Создано: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(8);})).GeneratePdf(s.FileName);MessageBox.Show("Отчёт PDF сохранён.");}catch(Exception ex){MessageBox.Show("Не удалось создать PDF: "+ex.Message);}}
}

internal sealed class TaskDialog : Form
{
    readonly string cs;readonly Session me;readonly long? id;readonly TextBox description=new(){Multiline=true,Width=440,Height=100};readonly ComboBox user=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=440};readonly ComboBox manager=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=440};readonly ComboBox status=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=440};
    public TaskDialog(string connectionString,Session session,long? taskId){cs=connectionString;me=session;id=taskId;Text=taskId.HasValue?"Изменить задачу":"Новая задача";Width=510;Height=470;StartPosition=FormStartPosition.CenterParent;var p=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,Padding=new Padding(18),WrapContents=false,AutoScroll=true};status.Items.AddRange(Db.Statuses.Cast<object>().ToArray());p.Controls.AddRange([new Label{Text="Описание (обязательно)",AutoSize=true,Margin=new Padding(0,2,0,5)},description,Ui.Divider(440),new Label{Text="Пользователь",AutoSize=true,Margin=new Padding(0,2,0,5)},user,Ui.Divider(440),new Label{Text="Менеджер",AutoSize=true,Margin=new Padding(0,2,0,5)},manager,Ui.Divider(440),new Label{Text="Статус",AutoSize=true,Margin=new Padding(0,2,0,5)},status]);var save=new Button{Text="Сохранить",Width=130};save.Click+=(_,_)=>Save();p.Controls.Add(save);Controls.Add(p);LoadLists();if(id.HasValue)LoadTaskData();else{status.SelectedItem="Новая";if(me.Role==Role.User){user.SelectedValue=me.Id;user.Enabled=false;}}Ui.Apply(this);}
    void LoadLists(){using var c=Db.Open(cs);var du=new DataTable();du.Load(new NpgsqlCommand("SELECT user_id,login FROM app_user ORDER BY login",c).ExecuteReader());user.DisplayMember="login";user.ValueMember="user_id";user.DataSource=du;var dm=new DataTable();dm.Load(new NpgsqlCommand("SELECT manager_id,login FROM manager ORDER BY login",c).ExecuteReader());manager.DisplayMember="login";manager.ValueMember="manager_id";manager.DataSource=dm;}
    void LoadTaskData(){using var c=Db.Open(cs);using var q=new NpgsqlCommand("SELECT status,description,user_id,manager_id FROM task WHERE task_id=@id",c);q.Parameters.AddWithValue("id",id!.Value);using var r=q.ExecuteReader();if(r.Read()){status.SelectedItem=Db.Decode(r.GetString(0));description.Text=Db.Decode(r.IsDBNull(1)?"":r.GetString(1));user.SelectedValue=r.GetInt64(2);manager.SelectedValue=r.GetInt64(3);}}
    void Save(){if(string.IsNullOrWhiteSpace(description.Text)||user.SelectedValue is null||manager.SelectedValue is null||status.SelectedItem is null){MessageBox.Show("Заполните описание и все обязательные списки.");return;}try{using var c=Db.Open(cs);using var tx=c.BeginTransaction();if(id.HasValue){using var q=new NpgsqlCommand("UPDATE task SET status=@s,description=@d,user_id=@u,manager_id=@m WHERE task_id=@id",c,tx);q.Parameters.AddWithValue("id",id.Value);q.Parameters.AddWithValue("s",status.Text);q.Parameters.AddWithValue("d",description.Text.Trim());q.Parameters.AddWithValue("u",Convert.ToInt64(user.SelectedValue));q.Parameters.AddWithValue("m",Convert.ToInt64(manager.SelectedValue));q.ExecuteNonQuery();}else{var taskId=Db.NextId(c,"task","task_id");using var q=new NpgsqlCommand("INSERT INTO task(task_id,status,description,created_at,user_id,manager_id) VALUES(@id,@s,@d,CURRENT_DATE,@u,@m)",c,tx);q.Parameters.AddWithValue("id",taskId);q.Parameters.AddWithValue("s","Новая");q.Parameters.AddWithValue("d",description.Text.Trim());q.Parameters.AddWithValue("u",me.Role==Role.User?me.Id:Convert.ToInt64(user.SelectedValue));q.Parameters.AddWithValue("m",Convert.ToInt64(manager.SelectedValue));q.ExecuteNonQuery();}tx.Commit();DialogResult=DialogResult.OK;Close();}catch(Exception ex){MessageBox.Show("Не удалось сохранить задачу: "+ex.Message);}}
}

internal sealed class WorkerForm : Form
{
    readonly string cs;readonly Session me;readonly DataGridView grid=new(){Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
    public WorkerForm(string connectionString,Session session){cs=connectionString;me=session;Text="Работники";Width=900;Height=550;var bar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=52,Padding=new Padding(10),BackColor=Color.White};var add=new Button{Text="Добавить"};var edit=new Button{Text="Изменить"};var del=new Button{Text="Удалить"};bar.Controls.AddRange([add,edit,del]);add.Click+=(_,_)=>Edit(null);edit.Click+=(_,_)=>{if(grid.CurrentRow!=null)Edit(Convert.ToInt64(grid.CurrentRow.Cells["worker_id"].Value));};del.Click+=(_,_)=>Delete();Controls.Add(grid);Controls.Add(bar);add.Visible=edit.Visible=del.Visible=me.Role==Role.Admin;LoadData();Ui.Apply(this);}
    void LoadData(){try{using var c=Db.Open(cs);var dt=new DataTable();var q=me.Role==Role.User?"SELECT worker_id,full_name,phone,experience,position,user_id FROM worker WHERE user_id=@id ORDER BY worker_id":"SELECT worker_id,full_name,phone,experience,position,user_id FROM worker ORDER BY worker_id";using var cmd=new NpgsqlCommand(q,c);if(me.Role==Role.User)cmd.Parameters.AddWithValue("id",me.Id);dt.Load(cmd.ExecuteReader());grid.DataSource=dt;string[] h=["ID","ФИО","Телефон","Стаж (лет)","Специальность","ID пользователя"];for(int i=0;i<h.Length;i++)grid.Columns[i].HeaderText=h[i];}catch(Exception ex){MessageBox.Show("Не удалось загрузить работников: "+ex.Message);}}
    void Edit(long? id){using var d=new WorkerDialog(cs,id);if(d.ShowDialog(this)==DialogResult.OK)LoadData();}
    void Delete(){if(grid.CurrentRow==null)return;var id=Convert.ToInt64(grid.CurrentRow.Cells[0].Value);if(MessageBox.Show("Удалить работника?","Подтверждение",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;try{using var c=Db.Open(cs);using var q=new NpgsqlCommand("DELETE FROM worker WHERE worker_id=@id",c);q.Parameters.AddWithValue("id",id);q.ExecuteNonQuery();LoadData();}catch(Exception ex){MessageBox.Show("Не удалось удалить работника: "+ex.Message);}}
}
internal sealed class WorkerDialog : Form
{
 readonly string cs;readonly long? id;readonly TextBox name=new(){Width=350},phone=new(){Width=350};readonly NumericUpDown experience=new(){Minimum=0,Maximum=70};readonly ComboBox position=new(){Width=350,DropDownStyle=ComboBoxStyle.DropDownList},user=new(){Width=350,DropDownStyle=ComboBoxStyle.DropDownList};
 public WorkerDialog(string connectionString,long? workerId){cs=connectionString;id=workerId;Text=id.HasValue?"Изменить работника":"Новый работник";Width=410;Height=440;var p=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,Padding=new Padding(15),WrapContents=false,AutoScroll=true};position.Items.AddRange(Db.Positions.Cast<object>().ToArray());using(var c=Db.Open(cs)){var dt=new DataTable();dt.Load(new NpgsqlCommand("SELECT user_id,login FROM app_user ORDER BY login",c).ExecuteReader());user.DisplayMember="login";user.ValueMember="user_id";user.DataSource=dt;}p.Controls.AddRange([new Label{Text="ФИО *"},name,new Label{Text="Телефон"},phone,Ui.Divider(350),new Label{Text="Стаж (лет)"},experience,new Label{Text="Специальность"},position,Ui.Divider(350),new Label{Text="Пользователь *"},user]);var b=new Button{Text="Сохранить"};b.Click+=(_,_)=>Save();p.Controls.Add(b);Controls.Add(p);if(id.HasValue)LoadWorkerData();Ui.Apply(this);}
 void LoadWorkerData(){using var c=Db.Open(cs);using var q=new NpgsqlCommand("SELECT full_name,phone,experience,position,user_id FROM worker WHERE worker_id=@id",c);q.Parameters.AddWithValue("id",id!.Value);using var r=q.ExecuteReader();if(r.Read()){name.Text=Db.Decode(r.GetString(0));phone.Text=r.IsDBNull(1)?"":r.GetString(1);experience.Value=r.GetInt32(2);position.SelectedItem=r.IsDBNull(3)?null:Db.Decode(r.GetString(3));user.SelectedValue=r.GetInt64(4);}}
 void Save(){if(string.IsNullOrWhiteSpace(name.Text)||user.SelectedValue is null){MessageBox.Show("ФИО и пользователь обязательны.");return;}try{using var c=Db.Open(cs);using var tx=c.BeginTransaction();var sql=id.HasValue?"UPDATE worker SET full_name=@n,phone=@p,experience=@e,\"position\"=@pos,user_id=@u WHERE worker_id=@id":"INSERT INTO worker(worker_id,full_name,phone,experience,\"position\",user_id) VALUES(@id,@n,@p,@e,@pos,@u)";var wid=id??Db.NextId(c,"worker","worker_id");using var q=new NpgsqlCommand(sql,c,tx);q.Parameters.AddWithValue("id",wid);q.Parameters.AddWithValue("n",name.Text.Trim());q.Parameters.AddWithValue("p",(object?)phone.Text.Trim()??DBNull.Value);q.Parameters.AddWithValue("e",(int)experience.Value);q.Parameters.AddWithValue("pos",(object?)position.SelectedItem??DBNull.Value);q.Parameters.AddWithValue("u",Convert.ToInt64(user.SelectedValue));q.ExecuteNonQuery();tx.Commit();DialogResult=DialogResult.OK;Close();}catch(Exception ex){MessageBox.Show("Не удалось сохранить работника: "+ex.Message);}}
}
internal sealed class AccountForm : Form
{
 readonly string cs;readonly DataGridView grid=new(){Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
 public AccountForm(string connectionString){cs=connectionString;Text="Пользователи и роли";Width=750;Height=500;var bar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=52,Padding=new Padding(10),BackColor=Color.White};var manager=new Button{Text="Создать менеджера"};manager.Click+=(_,_)=>CreateManager();bar.Controls.Add(manager);Controls.Add(grid);Controls.Add(bar);LoadData();Ui.Apply(this);}
 void LoadData(){using var c=Db.Open(cs);var dt=new DataTable();dt.Load(new NpgsqlCommand("SELECT user_id AS id,login,'Пользователь' AS role FROM app_user UNION ALL SELECT manager_id,login,'Менеджер' FROM manager UNION ALL SELECT admin_id,login,'Администратор' FROM admin ORDER BY role,login",c).ExecuteReader());grid.DataSource=dt;grid.Columns["id"].HeaderText="ID";grid.Columns["login"].HeaderText="Логин";grid.Columns["role"].HeaderText="Роль";}
 void CreateManager(){var l=Microsoft.VisualBasic.Interaction.InputBox("Логин менеджера (от 3 символов):","Новый менеджер").Trim();if(l.Length<3)return;var p=Microsoft.VisualBasic.Interaction.InputBox("Пароль (не короче 8 символов):","Новый менеджер");if(p.Length<8){MessageBox.Show("Пароль должен содержать минимум 8 символов.");return;}var n=Microsoft.VisualBasic.Interaction.InputBox("ФИО менеджера:","Новый менеджер").Trim();if(n.Length==0){MessageBox.Show("ФИО обязательно.");return;}try{using var c=Db.Open(cs);using var tx=c.BeginTransaction();var id=Db.NextId(c,"manager","manager_id");using var q=new NpgsqlCommand("INSERT INTO manager(manager_id,login,password,full_name) VALUES(@id,@l,@p,@n)",c,tx);q.Parameters.AddWithValue("id",id);q.Parameters.AddWithValue("l",l);q.Parameters.AddWithValue("p",Db.Hash(p));q.Parameters.AddWithValue("n",n);q.ExecuteNonQuery();tx.Commit();LoadData();}catch(Exception ex){MessageBox.Show("Не удалось создать менеджера: "+ex.Message);}}
}

internal sealed class ProfileForm : UserControl
{
    readonly string cs;readonly Session me;readonly Label fullName=new(){AutoSize=true},phone=new(){AutoSize=true},taskCount=new(){AutoSize=true};
    public ProfileForm(string connectionString,Session session)
    {
        cs=connectionString;me=session;Dock=DockStyle.Fill;BackColor=Ui.CanvasColor;
        var root=new Panel{Dock=DockStyle.Fill,Padding=new Padding(28),Tag="canvas"};var title=new Label{Text="Личный кабинет",AutoSize=true,Font=new Font("Segoe UI",22,FontStyle.Bold),Location=new Point(28,22)};var subtitle=new Label{Text="Данные вашей учётной записи",AutoSize=true,ForeColor=Ui.MutedColor,Tag="muted",Location=new Point(30,65)};
        var card=new RoundedPanel{Location=new Point(28,110),Size=new Size(520,240),BackColor=Ui.SurfaceColor,Radius=14,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};var avatar=new RoundedPanel{Location=new Point(22,22),Size=new Size(66,66),Radius=33,BackColor=Ui.AccentColor,Tag="accent"};var initials=new Label{Text=(session.Name.Length>0?session.Name[..1].ToUpperInvariant():"?").ToString(),ForeColor=Color.White,Font=new Font("Segoe UI",22,FontStyle.Bold),TextAlign=ContentAlignment.MiddleCenter,Dock=DockStyle.Fill,Tag="brand"};avatar.Controls.Add(initials);var nameCaption=new Label{Text=session.Name,AutoSize=true,Font=new Font("Segoe UI",15,FontStyle.Bold),Location=new Point(104,25)};var roleCaption=new Label{Text=RoleTitle(session.Role),AutoSize=true,ForeColor=Ui.MutedColor,Tag="muted",Location=new Point(106,54)};card.Controls.AddRange([avatar,nameCaption,roleCaption]);
        AddInfo(card,"Логин",session.Login,104);AddInfo(card,"ID аккаунта",session.Id.ToString(),145);AddInfo(card,"ФИО",fullName,186);AddInfo(card,"Телефон",phone,218);AddInfo(card,"Заявок",taskCount,250);
        var change=new RoundedButton{Text="Сменить пароль",Width=170,Height=40,Location=new Point(28,370),Anchor=AnchorStyles.Top|AnchorStyles.Left};change.Click+=(_,_)=>{using var dialog=new ChangePasswordDialog(cs,me);dialog.ShowDialog(this);};root.Controls.AddRange([title,subtitle,card,change]);Controls.Add(root);LoadProfile();Ui.Apply(this);
    }
    static string RoleTitle(Role role)=>role switch{Role.Admin=>"Администратор",Role.Manager=>"Менеджер",_=>"Пользователь"};
    static void AddInfo(Control card,string label,Control value,int y){var caption=new Label{Text=label,AutoSize=true,ForeColor=Ui.MutedColor,Tag="muted",Location=new Point(22,y)};value.Location=new Point(180,y);card.Controls.Add(caption);card.Controls.Add(value);}
    static void AddInfo(Control card,string label,string value,int y)=>AddInfo(card,label,new Label{Text=value,AutoSize=true},y);
    void LoadProfile(){try{using var c=Db.Open(cs);var table=me.Role switch{Role.Admin=>"admin",Role.Manager=>"manager",_=>"app_user"};var key=me.Role switch{Role.Admin=>"admin_id",Role.Manager=>"manager_id",_=>"user_id"};if(me.Role==Role.User){fullName.Text=me.Name;phone.Text="—";}else{using var cmd=new NpgsqlCommand($"SELECT full_name,phone FROM {table} WHERE {key}=@id",c);cmd.Parameters.AddWithValue("id",me.Id);using var r=cmd.ExecuteReader();if(r.Read()){fullName.Text=Db.Decode(r.IsDBNull(0)?me.Name:r.GetString(0));phone.Text=r.IsDBNull(1)||string.IsNullOrWhiteSpace(r.GetString(1))?"—":r.GetString(1);}}var countSql=me.Role switch{Role.User=>"SELECT COUNT(*) FROM task WHERE user_id=@id",Role.Manager=>"SELECT COUNT(*) FROM task WHERE manager_id=@id",_=>"SELECT COUNT(*) FROM task"};using var count=new NpgsqlCommand(countSql,c);if(me.Role!=Role.Admin)count.Parameters.AddWithValue("id",me.Id);taskCount.Text=Convert.ToString(count.ExecuteScalar())??"0";}catch(Exception ex){fullName.Text=me.Name;phone.Text="—";taskCount.Text="—";MessageBox.Show("Не удалось загрузить профиль: "+ex.Message);}}
}

internal sealed class ChangePasswordDialog : Form
{
    readonly string cs;readonly Session me;readonly TextBox current=new(){UseSystemPasswordChar=true,Width=320},next=new(){UseSystemPasswordChar=true,Width=320},confirm=new(){UseSystemPasswordChar=true,Width=320};
    public ChangePasswordDialog(string connectionString,Session session){cs=connectionString;me=session;Text="Смена пароля";Width=390;Height=300;StartPosition=FormStartPosition.CenterParent;var panel=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,Padding=new Padding(20)};panel.Controls.AddRange([new Label{Text="Текущий пароль"},current,new Label{Text="Новый пароль (от 8 символов)"},next,new Label{Text="Повторите новый пароль"},confirm]);var save=new RoundedButton{Text="Сохранить",Width=130,Height=38};save.Click+=(_,_)=>Save();panel.Controls.Add(save);Controls.Add(panel);Ui.Apply(this);}
    void Save(){if(next.Text.Length<8){MessageBox.Show("Новый пароль должен содержать не менее 8 символов.");return;}if(next.Text!=confirm.Text){MessageBox.Show("Повтор нового пароля не совпадает.");return;}var table=me.Role switch{Role.Admin=>"admin",Role.Manager=>"manager",_=>"app_user"};var key=me.Role switch{Role.Admin=>"admin_id",Role.Manager=>"manager_id",_=>"user_id"};try{using var c=Db.Open(cs);using var read=new NpgsqlCommand($"SELECT password FROM {table} WHERE {key}=@id",c);read.Parameters.AddWithValue("id",me.Id);var stored=read.ExecuteScalar() as string;if(stored is null||!Db.Verify(current.Text,stored)){MessageBox.Show("Текущий пароль введён неверно.");return;}using var update=new NpgsqlCommand($"UPDATE {table} SET password=@password WHERE {key}=@id",c);update.Parameters.AddWithValue("password",Db.Hash(next.Text));update.Parameters.AddWithValue("id",me.Id);update.ExecuteNonQuery();MessageBox.Show("Пароль изменён.");DialogResult=DialogResult.OK;Close();}catch(Exception ex){MessageBox.Show("Не удалось изменить пароль: "+ex.Message);}}
}
