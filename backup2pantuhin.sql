--
-- PostgreSQL database dump
--

\restrict LJdLNs6sgu0Adwn8oqQeZ4k7pBA5GAWhwsfcx2cYrxckeSxLeRFVfmcbDuYgXpb

-- Dumped from database version 18.6 (Debian 18.6-1.pgdg13+2)
-- Dumped by pg_dump version 18.4

-- Started on 2026-10-29 15:21:08

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 16527)
-- Name: admin; Type: TABLE; Schema: public; Owner: user1
--

CREATE TABLE public.admin (
    admin_id bigint NOT NULL,
    login character varying(50) NOT NULL,
    password character varying(100) NOT NULL,
    phone character varying(20),
    full_name character varying(50) NOT NULL
);


ALTER TABLE public.admin OWNER TO user1;

--
-- TOC entry 220 (class 1259 OID 16534)
-- Name: app_user; Type: TABLE; Schema: public; Owner: user1
--

CREATE TABLE public.app_user (
    user_id bigint NOT NULL,
    login character varying(50) NOT NULL,
    password character varying(100) NOT NULL,
    admin_id bigint
);


ALTER TABLE public.app_user OWNER TO user1;

--
-- TOC entry 221 (class 1259 OID 16540)
-- Name: manager; Type: TABLE; Schema: public; Owner: user1
--

CREATE TABLE public.manager (
    manager_id bigint NOT NULL,
    login character varying(50) NOT NULL,
    password character varying(100) NOT NULL,
    full_name character varying(50) NOT NULL,
    phone character varying(20)
);


ALTER TABLE public.manager OWNER TO user1;

--
-- TOC entry 222 (class 1259 OID 16547)
-- Name: task; Type: TABLE; Schema: public; Owner: user1
--

CREATE TABLE public.task (
    task_id bigint NOT NULL,
    status character varying(20) DEFAULT 'Новая'::character varying NOT NULL,
    description text,
    created_at date DEFAULT CURRENT_DATE NOT NULL,
    user_id bigint NOT NULL,
    manager_id bigint NOT NULL
);


ALTER TABLE public.task OWNER TO user1;

--
-- TOC entry 223 (class 1259 OID 16559)
-- Name: worker; Type: TABLE; Schema: public; Owner: user1
--

CREATE TABLE public.worker (
    worker_id bigint NOT NULL,
    full_name character varying(50) NOT NULL,
    phone character varying(20),
    experience integer DEFAULT 0 NOT NULL,
    "position" character varying(50),
    user_id bigint NOT NULL,
    CONSTRAINT worker_experience_check CHECK ((experience >= 0))
);


ALTER TABLE public.worker OWNER TO user1;

--
-- TOC entry 3475 (class 0 OID 16527)
-- Dependencies: 219
-- Data for Name: admin; Type: TABLE DATA; Schema: public; Owner: user1
--

COPY public.admin (admin_id, login, password, phone, full_name) FROM stdin;
1	admin1	pass1	Ярослав Попов	+79671488337
\.


--
-- TOC entry 3476 (class 0 OID 16534)
-- Dependencies: 220
-- Data for Name: app_user; Type: TABLE DATA; Schema: public; Owner: user1
--

COPY public.app_user (user_id, login, password, admin_id) FROM stdin;
1	user001	password001	\N
2	user002	password002	\N
3	user003	password003	\N
4	user004	password004	\N
5	user005	password005	\N
6	user006	password006	\N
7	user007	password007	\N
8	user008	password008	\N
9	user009	password009	\N
10	user010	password010	\N
11	user011	password011	\N
12	user012	password012	\N
13	user013	password013	\N
14	user014	password014	\N
15	user015	password015	\N
16	user016	password016	\N
17	user017	password017	\N
18	user018	password018	\N
19	user019	password019	\N
20	user020	password020	\N
21	user021	password021	\N
22	user022	password022	\N
23	user023	password023	\N
24	user024	password024	\N
25	user025	password025	\N
26	user026	password026	\N
27	user027	password027	\N
28	user028	password028	\N
29	user029	password029	\N
30	user030	password030	\N
31	user031	password031	\N
32	user032	password032	\N
33	user033	password033	\N
34	user034	password034	\N
35	user035	password035	\N
36	user036	password036	\N
37	user037	password037	\N
38	user038	password038	\N
39	user039	password039	\N
40	user040	password040	\N
41	user041	password041	\N
42	user042	password042	\N
43	user043	password043	\N
44	user044	password044	\N
45	user045	password045	\N
46	user046	password046	\N
47	user047	password047	\N
48	user048	password048	\N
49	user049	password049	\N
50	user050	password050	\N
51	user051	password051	\N
52	user052	password052	\N
53	user053	password053	\N
54	user054	password054	\N
55	user055	password055	\N
56	user056	password056	\N
57	user057	password057	\N
58	user058	password058	\N
59	user059	password059	\N
60	user060	password060	\N
61	user061	password061	\N
62	user062	password062	\N
63	user063	password063	\N
64	user064	password064	\N
65	user065	password065	\N
66	user066	password066	\N
67	user067	password067	\N
68	user068	password068	\N
69	user069	password069	\N
70	user070	password070	\N
71	user071	password071	\N
72	user072	password072	\N
73	user073	password073	\N
74	user074	password074	\N
75	user075	password075	\N
76	user076	password076	\N
77	user077	password077	\N
78	user078	password078	\N
79	user079	password079	\N
80	user080	password080	\N
81	user081	password081	\N
82	user082	password082	\N
83	user083	password083	\N
84	user084	password084	\N
85	user085	password085	\N
86	user086	password086	\N
87	user087	password087	\N
88	user088	password088	\N
89	user089	password089	\N
90	user090	password090	\N
91	user091	password091	\N
92	user092	password092	\N
93	user093	password093	\N
94	user094	password094	\N
95	user095	password095	\N
96	user096	password096	\N
97	user097	password097	\N
98	user098	password098	\N
99	user099	password099	\N
100	user100	password100	\N
\.


--
-- TOC entry 3477 (class 0 OID 16540)
-- Dependencies: 221
-- Data for Name: manager; Type: TABLE DATA; Schema: public; Owner: user1
--

COPY public.manager (manager_id, login, password, full_name, phone) FROM stdin;
1	manager1	pass1	Рафаэль Хасанов	+79231235567
\.


--
-- TOC entry 3478 (class 0 OID 16547)
-- Dependencies: 222
-- Data for Name: task; Type: TABLE DATA; Schema: public; Owner: user1
--

COPY public.task (task_id, status, description, created_at, user_id, manager_id) FROM stdin;
2	В работе	Проверить логи сервера для юзера №2	2026-10-01	2	1
3	Выполнена	Обновить документацию проекта	2026-10-01	3	1
4	Новая	Созвониться с клиентом	2026-10-01	4	1
5	В работе	Исправить баг в коде	2026-10-02	5	1
6	Новая	Провести ревью пул-реквеста	2026-10-02	6	1
7	Выполнена	Настроить CI/CD пайплайн	2026-10-02	7	1
8	Новая	Оптимизировать SQL-запросы	2026-10-02	8	1
9	В работе	Протестировать новый функционал	2026-10-03	9	1
10	Отменена	Собрать обратную связь	2026-10-03	10	1
11	Новая	Подготовить отчет по задаче №11	2026-10-03	11	1
12	В работе	Проверить логи сервера для юзера №12	2026-10-03	12	1
13	Выполнена	Обновить документацию проекта	2026-10-04	13	1
14	Новая	Созвониться с клиентом	2026-10-04	14	1
15	В работе	Исправить баг в коде	2026-10-04	15	1
16	Новая	Провести ревью пул-реквеста	2026-10-04	16	1
17	Выполнена	Настроить CI/CD пайплайн	2026-10-05	17	1
18	Новая	Оптимизировать SQL-запросы	2026-10-05	18	1
19	В работе	Протестировать новый функционал	2026-10-05	19	1
20	Отменена	Собрать обратную связь	2026-10-05	20	1
21	Новая	Подготовить отчет по задаче №21	2026-10-06	21	1
22	В работе	Проверить логи сервера для юзера №22	2026-10-06	22	1
23	Выполнена	Обновить документацию проекта	2026-10-06	23	1
24	Новая	Созвониться с клиентом	2026-10-06	24	1
25	В работе	Исправить баг в коде	2026-10-07	25	1
26	Новая	Провести ревью пул-реквеста	2026-10-07	26	1
27	Выполнена	Настроить CI/CD пайплайн	2026-10-07	27	1
28	Новая	Оптимизировать SQL-запросы	2026-10-07	28	1
29	В работе	Протестировать новый функционал	2026-10-08	29	1
30	Отменена	Собрать обратную связь	2026-10-08	30	1
31	Новая	Подготовить отчет по задаче №31	2026-10-08	31	1
32	В работе	Проверить логи сервера для юзера №32	2026-10-08	32	1
33	Выполнена	Обновить документацию проекта	2026-10-09	33	1
34	Новая	Созвониться с клиентом	2026-10-09	34	1
35	В работе	Исправить баг в коде	2026-10-09	35	1
36	Новая	Провести ревью пул-реквеста	2026-10-09	36	1
37	Выполнена	Настроить CI/CD пайплайн	2026-10-10	37	1
38	Новая	Оптимизировать SQL-запросы	2026-10-10	38	1
39	В работе	Протестировать новый функционал	2026-10-10	39	1
40	Отменена	Собрать обратную связь	2026-10-10	40	1
41	Новая	Подготовить отчет по задаче №41	2026-10-11	41	1
42	В работе	Проверить логи сервера для юзера №42	2026-10-11	42	1
43	Выполнена	Обновить документацию проекта	2026-10-11	43	1
44	Новая	Созвониться с клиентом	2026-10-11	44	1
45	В работе	Исправить баг в коде	2026-10-12	45	1
46	Новая	Провести ревью пул-реквеста	2026-10-12	46	1
47	Выполнена	Настроить CI/CD пайплайн	2026-10-12	47	1
48	Новая	Оптимизировать SQL-запросы	2026-10-12	48	1
49	В работе	Протестировать новый функционал	2026-10-13	49	1
50	Отменена	Собрать обратную связь	2026-10-13	50	1
51	Новая	Подготовить отчет по задаче №51	2026-10-13	51	1
52	В работе	Проверить логи сервера для юзера №52	2026-10-13	52	1
53	Выполнена	Обновить документацию проекта	2026-10-14	53	1
54	Новая	Созвониться с клиентом	2026-10-14	54	1
55	В работе	Исправить баг в коде	2026-10-14	55	1
56	Новая	Провести ревью пул-реквеста	2026-10-14	56	1
57	Выполнена	Настроить CI/CD пайплайн	2026-10-15	57	1
58	Новая	Оптимизировать SQL-запросы	2026-10-15	58	1
59	В работе	Протестировать новый функционал	2026-10-15	59	1
60	Отменена	Собрать обратную связь	2026-10-15	60	1
61	Новая	Подготовить отчет по задаче №61	2026-10-16	61	1
62	В работе	Проверить логи сервера для юзера №62	2026-10-16	62	1
63	Выполнена	Обновить документацию проекта	2026-10-16	63	1
64	Новая	Созвониться с клиентом	2026-10-16	64	1
65	В работе	Исправить баг в коде	2026-10-17	65	1
66	Новая	Провести ревью пул-реквеста	2026-10-17	66	1
67	Выполнена	Настроить CI/CD пайплайн	2026-10-17	67	1
68	Новая	Оптимизировать SQL-запросы	2026-10-17	68	1
69	В работе	Протестировать новый функционал	2026-10-18	69	1
70	Отменена	Собрать обратную связь	2026-10-18	70	1
71	Новая	Подготовить отчет по задаче №71	2026-10-18	71	1
72	В работе	Проверить логи сервера для юзера №72	2026-10-18	72	1
73	Выполнена	Обновить документацию проекта	2026-10-19	73	1
74	Новая	Созвониться с клиентом	2026-10-19	74	1
75	В работе	Исправить баг в коде	2026-10-19	75	1
76	Новая	Провести ревью пул-реквеста	2026-10-19	76	1
77	Выполнена	Настроить CI/CD пайплайн	2026-10-20	77	1
78	Новая	Оптимизировать SQL-запросы	2026-10-20	78	1
79	В работе	Протестировать новый функционал	2026-10-20	79	1
80	Отменена	Собрать обратную связь	2026-10-20	80	1
81	Новая	Подготовить отчет по задаче №81	2026-10-21	81	1
82	В работе	Проверить логи сервера для юзера №82	2026-10-21	82	1
83	Выполнена	Обновить документацию проекта	2026-10-21	83	1
84	Новая	Созвониться с клиентом	2026-10-21	84	1
85	В работе	Исправить баг в коде	2026-10-22	85	1
86	Новая	Провести ревью пул-реквеста	2026-10-22	86	1
87	Выполнена	Настроить CI/CD пайплайн	2026-10-22	87	1
88	Новая	Оптимизировать SQL-запросы	2026-10-22	88	1
89	В работе	Протестировать новый функционал	2026-10-23	89	1
90	Отменена	Собрать обратную связь	2026-10-23	90	1
91	Новая	Подготовить отчет по задаче №91	2026-10-24	91	1
92	В работе	Проверить логи сервера для юзера №92	2026-10-24	92	1
93	Выполнена	Обновить документацию проекта	2026-10-24	93	1
94	Новая	Созвониться с клиентом	2026-10-24	94	1
95	В работе	Исправить баг в коде	2026-10-25	95	1
96	Новая	Провести ревью пул-реквеста	2026-10-25	96	1
97	Выполнена	Настроить CI/CD пайплайн	2026-10-25	97	1
98	Новая	Оптимизировать SQL-запросы	2026-10-25	98	1
99	В работе	Протестировать новый функционал	2026-10-26	99	1
100	Новая	Решить задачу №100	2026-10-26	100	1
1	В работе	Подготовить отчет по задаче №1	2026-10-01	1	1
\.


--
-- TOC entry 3479 (class 0 OID 16559)
-- Dependencies: 223
-- Data for Name: worker; Type: TABLE DATA; Schema: public; Owner: user1
--

COPY public.worker (worker_id, full_name, phone, experience, "position", user_id) FROM stdin;
1	Иван Петров	+79000000001	1	Электрик	1
2	Алексей Иванов	+79000000002	3	Сантехник	2
3	Дмитрий Смирнов	+79000000003	5	Плотник	3
4	Сергей Кузнецов	+79000000004	2	Маляр	4
5	Андрей Попов	+79000000005	7	Сварщик	5
6	Максим Васильев	+79000000006	4	Электрик	6
7	Николай Соколов	+79000000007	6	Сантехник	7
8	Владимир Михайлов	+79000000008	8	Плиточник	8
9	Павел Новиков	+79000000009	1	Разнорабочий	9
10	Артем Федоров	+79000000010	9	Прораб	10
11	Игорь Морозов	+79000000011	2	Плотник	11
12	Олег Волков	+79000000012	5	Маляр	12
13	Роман Алексеев	+79000000013	3	Сварщик	13
14	Кирилл Лебедев	+79000000014	4	Электрик	14
15	Евгений Семенов	+79000000015	6	Сантехник	15
16	Антон Егоров	+79000000016	7	Плиточник	16
17	Михаил Павлов	+79000000017	1	Разнорабочий	17
18	Григорий Козлов	+79000000018	10	Прораб	18
19	Степан Степанов	+79000000019	2	Плотник	19
20	Виктор Орлов	+79000000020	5	Маляр	20
21	Денис Андреев	+79000000021	3	Сварщик	21
22	Юрий Макаров	+79000000022	4	Электрик	22
23	Константин Никитин	+79000000023	8	Сантехник	23
24	Вячеслав Захаров	+79000000024	6	Плиточник	24
25	Аркадий Зайцев	+79000000025	1	Разнорабочий	25
26	Борис Соловьев	+79000000026	9	Прораб	26
27	Георгий Борисов	+79000000027	2	Плотник	27
28	Руслан Яковлев	+79000000028	5	Маляр	28
29	Тимур Григорьев	+79000000029	7	Сварщик	29
30	Федор Романов	+79000000030	4	Электрик	30
31	Леонид Воробьев	+79000000031	6	Сантехник	31
32	Станислав Сергеев	+79000000032	8	Плиточник	32
33	Валерий Кузьмин	+79000000033	1	Разнорабочий	33
34	Анатолий Фролов	+79000000034	10	Прораб	34
35	Матвей Александров	+79000000035	3	Плотник	35
36	Даниил Дмитриев	+79000000036	5	Маляр	36
37	Глеб Королев	+79000000037	2	Сварщик	37
38	Ярослав Гусев	+79000000038	4	Электрик	38
39	Тимофей Киселев	+79000000039	6	Сантехник	39
40	Вадим Ильин	+79000000040	7	Плиточник	40
41	Егор Максимов	+79000000041	1	Разнорабочий	41
42	Семен Поляков	+79000000042	9	Прораб	42
43	Артур Сорокин	+79000000043	2	Плотник	43
44	Марк Виноградов	+79000000044	5	Маляр	44
45	Лев Ковалев	+79000000045	3	Сварщик	45
46	Никита Белов	+79000000046	4	Электрик	46
47	Петр Медведев	+79000000047	8	Сантехник	47
48	Виталий Антонов	+79000000048	6	Плиточник	48
49	Захар Тарасов	+79000000049	1	Разнорабочий	49
50	Илья Жуков	+79000000050	10	Прораб	50
51	Адам Баранов	+79000000051	2	Плотник	51
52	Александр Комаров	+79000000052	5	Маляр	52
53	Василий Давыдов	+79000000053	7	Сварщик	53
54	Геннадий Беляев	+79000000054	4	Электрик	54
55	Давид Герасимов	+79000000055	6	Сантехник	55
56	Елисей Богданов	+79000000056	8	Плиточник	56
57	Зиновий Осипов	+79000000057	1	Разнорабочий	57
58	Клим Сидоров	+79000000058	9	Прораб	58
59	Лука Матвеев	+79000000059	3	Плотник	59
60	Мирон Титов	+79000000060	5	Маляр	60
61	Назар Марков	+79000000061	2	Сварщик	61
62	Платон Миронов	+79000000062	4	Электрик	62
63	Прохор Крылов	+79000000063	6	Сантехник	63
64	Родион Калашников	+79000000064	7	Плиточник	64
65	Святослав Афанасьев	+79000000065	1	Разнорабочий	65
66	Трофим Фомин	+79000000066	10	Прораб	66
67	Филипп Наумов	+79000000067	2	Плотник	67
68	Эдуард Басов	+79000000068	5	Маляр	68
69	Юлиан Абрамов	+79000000069	3	Сварщик	69
70	Ян Мельников	+79000000070	4	Электрик	70
71	Викентий Денисов	+79000000071	8	Сантехник	71
72	Герман Власов	+79000000072	6	Плиточник	72
73	Демьян Маслов	+79000000073	1	Разнорабочий	73
74	Еремей Исаков	+79000000074	9	Прораб	74
75	Игнат Тихонов	+79000000075	2	Плотник	75
76	Кузьма Котов	+79000000076	5	Маляр	76
77	Лаврентий Горбунов	+79000000077	7	Сварщик	77
78	Макар Савельев	+79000000078	4	Электрик	78
79	Нестор Панфилов	+79000000079	6	Сантехник	79
80	Остап Ершов	+79000000080	8	Плиточник	80
81	Ренат Ширяев	+79000000081	1	Разнорабочий	81
82	Самсон Филатов	+79000000082	10	Прораб	82
83	Тимур Капустин	+79000000083	3	Плотник	83
84	Феликс Муравьев	+79000000084	5	Маляр	84
85	Эмиль Рогов	+79000000085	2	Сварщик	85
86	Юрий Селезнев	+79000000086	4	Электрик	86
87	Яков Чернов	+79000000087	6	Сантехник	87
88	Вениамин Голубев	+79000000088	7	Плиточник	88
89	Гавриил Харитонов	+79000000089	1	Разнорабочий	89
90	Даниэль Шестаков	+79000000090	9	Прораб	90
91	Ефим Казаков	+79000000091	2	Плотник	91
92	Иосиф Федосеев	+79000000092	5	Маляр	92
93	Кирилл Одинцов	+79000000093	3	Сварщик	93
94	Леон Горшков	+79000000094	4	Электрик	94
95	Марат Зимин	+79000000095	8	Сантехник	95
96	Никифор Лапин	+79000000096	6	Плиточник	96
97	Оскар Рябов	+79000000097	1	Разнорабочий	97
98	Павел Суворов	+79000000098	10	Прораб	98
99	Роберт Носов	+79000000099	2	Плотник	99
100	Савелий Устинов	+79000000100	5	Маляр	100
\.


--
-- TOC entry 3309 (class 2606 OID 16569)
-- Name: admin admin_login_key; Type: CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.admin
    ADD CONSTRAINT admin_login_key UNIQUE (login);


--
-- TOC entry 3311 (class 2606 OID 16571)
-- Name: admin admin_pkey; Type: CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.admin
    ADD CONSTRAINT admin_pkey PRIMARY KEY (admin_id);


--
-- TOC entry 3313 (class 2606 OID 16573)
-- Name: app_user app_user_login_key; Type: CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.app_user
    ADD CONSTRAINT app_user_login_key UNIQUE (login);


--
-- TOC entry 3315 (class 2606 OID 16575)
-- Name: app_user app_user_pkey; Type: CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.app_user
    ADD CONSTRAINT app_user_pkey PRIMARY KEY (user_id);


--
-- TOC entry 3317 (class 2606 OID 16577)
-- Name: manager manager_login_key; Type: CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.manager
    ADD CONSTRAINT manager_login_key UNIQUE (login);


--
-- TOC entry 3319 (class 2606 OID 16579)
-- Name: manager manager_pkey; Type: CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.manager
    ADD CONSTRAINT manager_pkey PRIMARY KEY (manager_id);


--
-- TOC entry 3321 (class 2606 OID 16581)
-- Name: task task_pkey; Type: CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.task
    ADD CONSTRAINT task_pkey PRIMARY KEY (task_id);


--
-- TOC entry 3323 (class 2606 OID 16583)
-- Name: worker worker_pkey; Type: CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.worker
    ADD CONSTRAINT worker_pkey PRIMARY KEY (worker_id);


--
-- TOC entry 3324 (class 2606 OID 16584)
-- Name: app_user app_user_admin_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.app_user
    ADD CONSTRAINT app_user_admin_id_fkey FOREIGN KEY (admin_id) REFERENCES public.admin(admin_id);


--
-- TOC entry 3325 (class 2606 OID 16589)
-- Name: task task_manager_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.task
    ADD CONSTRAINT task_manager_id_fkey FOREIGN KEY (manager_id) REFERENCES public.manager(manager_id);


--
-- TOC entry 3326 (class 2606 OID 16594)
-- Name: task task_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.task
    ADD CONSTRAINT task_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(user_id);


--
-- TOC entry 3327 (class 2606 OID 16599)
-- Name: worker worker_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: user1
--

ALTER TABLE ONLY public.worker
    ADD CONSTRAINT worker_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.app_user(user_id);


-- Completed on 2026-10-29 15:21:08

--
-- PostgreSQL database dump complete
--

\unrestrict LJdLNs6sgu0Adwn8oqQeZ4k7pBA5GAWhwsfcx2cYrxckeSxLeRFVfmcbDuYgXpb

