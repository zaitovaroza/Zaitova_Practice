-- Скрипт создания базы данных для подсистемы работы с партнерами
-- Фамилия студента: Zaitova
-- База данных: Zaitova_BD

-- 1. Создание роли
CREATE ROLE app WITH LOGIN PASSWORD '123456789';

-- 2. Создание базы данных
CREATE DATABASE Zaitova_BD
    OWNER = app
    ENCODING = 'UTF8'
    TABLESPACE = pg_default;

-- Подключение к базе данных Zaitova_BD
\c Zaitova_BD;

-- 3. Создание схемы app
CREATE SCHEMA IF NOT EXISTS app AUTHORIZATION app;

-- 4. Установка поискового пути
SET search_path TO app;

-- 5. Предоставление прав
GRANT ALL ON SCHEMA app TO app;
GRANT ALL PRIVILEGES ON DATABASE Zaitova_BD TO app;

-- 6. Создание таблиц (3 нормальная форма)

-- Справочник типов партнеров
CREATE TABLE app.partner_types_zaitova (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT
);

-- Таблица партнеров
CREATE TABLE app.partners_zaitova (
    id SERIAL PRIMARY KEY,
    type_id INTEGER NOT NULL REFERENCES app.partner_types_zaitova(id) ON DELETE RESTRICT,
    company_name VARCHAR(255) NOT NULL,
    legal_address TEXT,
    inn VARCHAR(12) UNIQUE,
    director_fullname VARCHAR(255),
    phone VARCHAR(20),
    email VARCHAR(100),
    rating INTEGER NOT NULL DEFAULT 0 CHECK (rating >= 0 AND rating <= 100),
    logo_path TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица продукции
CREATE TABLE app.products_zaitova (
    id SERIAL PRIMARY KEY,
    article VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    min_price DECIMAL(15, 2),
    unit VARCHAR(20) DEFAULT 'шт',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица истории продаж
CREATE TABLE app.sales_history_zaitova (
    id SERIAL PRIMARY KEY,
    partner_id INTEGER NOT NULL REFERENCES app.partners_zaitova(id) ON DELETE CASCADE,
    product_id INTEGER NOT NULL REFERENCES app.products_zaitova(id) ON DELETE RESTRICT,
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    sale_date DATE NOT NULL,
    total_amount DECIMAL(15, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица мест продаж
CREATE TABLE app.sales_points_zaitova (
    id SERIAL PRIMARY KEY,
    partner_id INTEGER NOT NULL REFERENCES app.partners_zaitova(id) ON DELETE CASCADE,
    address TEXT NOT NULL,
    point_type VARCHAR(50)
);

-- Индексы для оптимизации
CREATE INDEX idx_partners_type ON app.partners_zaitova(type_id);
CREATE INDEX idx_sales_partner ON app.sales_history_zaitova(partner_id);
CREATE INDEX idx_sales_product ON app.sales_history_zaitova(product_id);
CREATE INDEX idx_sales_date ON app.sales_history_zaitova(sale_date);

-- Функция обновления updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Триггер для автоматического обновления updated_at
CREATE TRIGGER update_partners_updated_at
    BEFORE UPDATE ON app.partners_zaitova
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Заполнение начальными данными
INSERT INTO app.partner_types_zaitova (name, description) VALUES
    ('ООО', 'Общество с ограниченной ответственностью'),
    ('ИП', 'Индивидуальный предприниматель'),
    ('ЗАО', 'Закрытое акционерное общество'),
    ('ОАО', 'Открытое акционерное общество'),
    ('ЧУП', 'Частное унитарное предприятие');

INSERT INTO app.products_zaitova (article, name, description, min_price, unit) VALUES
    ('MP-001', 'Ламинат Дуб благородный', 'Ламинат 32 класс, толщина 8 мм', 850.00, 'м²'),
    ('MP-002', 'Паркетная доска Ясень', 'Паркетная доска трехслойная, 14 мм', 2100.00, 'м²'),
    ('MP-003', 'Виниловая плитка', 'Кварц-винил, замковое соединение', 1200.00, 'м²'),
    ('MP-004', 'Плинтус напольный', 'Пластиковый, цвет белый', 150.00, 'шт'),
    ('MP-005', 'Подложка пробковая', 'Подложка 2 мм, рулон 10 м²', 300.00, 'рулон');

-- Добавляем партнеров из Перми и Пермского края
INSERT INTO app.partners_zaitova (type_id, company_name, legal_address, inn, director_fullname, phone, email, rating) VALUES
    (1, 'ООО "ПермьПаркет"', 'г. Пермь, ул. Мира, 25', '5904123456', 'Кузнецов Андрей Петрович', '+7(342)111-22-33', 'info@permparket.ru', 94),
    (2, 'ИП Соболева Е.В.', 'г. Пермь, ул. Ленина, 52', '5904234567', 'Соболева Елена Владимировна', '+7(912)333-44-55', 'soboleva@mail.ru', 72),
    (1, 'ООО "УралПол"', 'г. Пермь, Комсомольский пр., 34', '5904345678', 'Мальцев Сергей Александрович', '+7(342)555-66-77', 'uralpol@bk.ru', 85),
    (3, 'ЗАО "СтройКомплект"', 'г. Березники, ул. Пятилетки, 18', '5904456789', 'Воронова Татьяна Игоревна', '+7(342)777-88-99', 'stroykom@mail.ru', 91),
    (2, 'ИП Чайкин Д.А.', 'г. Соликамск, ул. Советская, 41', '5904567890', 'Чайкин Дмитрий Алексеевич', '+7(912)666-77-88', 'chaikin@bk.ru', 67),
    (1, 'ООО "ПармаСтрой"', 'г. Кунгур, ул. Карла Маркса, 12', '5904678901', 'Одинцов Павел Валерьевич', '+7(342)999-00-11', 'parma@yandex.ru', 79),
    (2, 'ИП Козлова Н.С.', 'г. Лысьва, ул. Металлистов, 8', '5904789012', 'Козлова Наталья Сергеевна', '+7(902)111-22-33', 'kozlova@mail.ru', 63);

-- Добавляем историю продаж
INSERT INTO app.sales_history_zaitova (partner_id, product_id, quantity, sale_date, total_amount) VALUES
    -- ООО "ПермьПаркет" (id=1)
    (1, 1, 150, '2026-01-10', 127500.00),
    (1, 2, 60, '2026-01-15', 126000.00),
    (1, 3, 40, '2026-01-25', 48000.00),
    (1, 4, 300, '2026-02-01', 45000.00),
    (1, 1, 50, '2026-02-10', 42500.00),
    
    -- ИП Соболева Е.В. (id=2)
    (2, 3, 50, '2026-01-12', 60000.00),
    (2, 5, 100, '2026-01-20', 30000.00),
    (2, 1, 40, '2026-02-05', 34000.00),
    (2, 4, 20, '2026-02-15', 3000.00),
    
    -- ООО "УралПол" (id=3)
    (3, 2, 45, '2026-01-08', 94500.00),
    (3, 1, 60, '2026-01-18', 51000.00),
    (3, 4, 200, '2026-01-28', 30000.00),
    (3, 3, 40, '2026-02-07', 48000.00),
    
    -- ЗАО "СтройКомплект" (id=4)
    (4, 2, 150, '2026-01-05', 315000.00),
    (4, 1, 200, '2026-01-22', 170000.00),
    (4, 3, 80, '2026-02-03', 96000.00),
    (4, 2, 50, '2026-02-12', 105000.00),
    (4, 4, 10, '2026-02-18', 1500.00),
    
    -- ИП Чайкин Д.А. (id=5)
    (5, 5, 60, '2026-01-14', 18000.00),
    (5, 4, 100, '2026-01-24', 15000.00),
    (5, 3, 5, '2026-02-09', 6000.00),
    
    -- ООО "ПармаСтрой" (id=6)
    (6, 1, 120, '2026-01-17', 102000.00),
    (6, 2, 50, '2026-01-27', 105000.00),
    (6, 3, 30, '2026-02-06', 36000.00),
    (6, 4, 280, '2026-02-14', 42000.00),
    
    -- ИП Козлова Н.С. (id=7)
    (7, 5, 40, '2026-01-21', 12000.00),
    (7, 4, 30, '2026-02-11', 4500.00);