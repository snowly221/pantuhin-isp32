SELECT w.worker_id, w.full_name, w.phone, w.experience,
       w.position, u.login
FROM worker w
JOIN app_user u ON u.user_id = w.user_id
ORDER BY w.worker_id;

SELECT t.task_id, t.status, t.description, t.created_at,
       u.login AS user_login, m.full_name AS manager_name
FROM task t
JOIN app_user u ON u.user_id = t.user_id
JOIN manager m ON m.manager_id = t.manager_id
ORDER BY t.created_at DESC;

UPDATE task
SET status = 'В работе'
WHERE task_id = 1
RETURNING *;

SELECT *
FROM task
WHERE status = 'Новая'
ORDER BY created_at DESC;

SELECT status, COUNT(*) AS task_count
FROM task
GROUP BY status
ORDER BY task_count DESC;

SELECT worker_id, full_name, phone, experience, position
FROM worker
WHERE experience >= 3
ORDER BY experience DESC;

SELECT * FROM task WHERE task_id = 1;
