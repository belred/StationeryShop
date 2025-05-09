﻿using StationeryShop.Models;

namespace StationeryShop.Data
{
    public static class SeedData
    {
        public static void Initialize(StationeryDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Синяя шариковая ручка",
                        Description = "Удобная шариковая ручка с синими чернилами. Плавное письмо, эргономичный корпус. Идеальна для повседневного использования.",
                        Price = 55
                    },
                    new Product
                    {
                        Name = "Чёрная шариковая ручка",
                        Description = "Стильная чёрная шариковая ручка с насыщенными чернилами. Надёжный механизм, комфортный захват. Подходит для документов и заметок.",
                        Price = 60
                    },
                    new Product
                    {
                        Name = "Красная шариковая ручка",
                        Description = "Яркая красная шариковая ручка для выделения важных записей. Чёткие линии, стойкие чернила. Отлично подходит для преподавателей и редакторов.",
                        Price = 65
                    },
                    new Product
                    {
                        Name = "Зелёная шариковая ручка",
                        Description = "Экологичная зелёная шариковая ручка с мягким письмом. Приятный дизайн, лёгкость в использовании. Хороший выбор для творческих людей.",
                        Price = 65
                    },
                    new Product
                    {
                        Name = "Тетрадь в клетку 48 листов",
                        Description = "Тетрадь в клетку с 48 листами высококачественной бумаги. Плотная обложка защищает от повреждений. Универсальный вариант для школы и работы.",
                        Price = 130
                    },
                    new Product
                    {
                        Name = "Тетрадь в клетку 24 листа",
                        Description = "Компактная тетрадь в клетку на 24 листа. Удобный формат для коротких заметок. Яркая обложка с защитой от загрязнений.",
                        Price = 100
                    },
                    new Product
                    {
                        Name = "Тетрадь в клетку 18 листов",
                        Description = "Лёгкая тетрадь в клетку на 18 листов. Идеальна для быстрых записей и черновиков. Экономичный и практичный вариант.",
                        Price = 85
                    },
                    new Product
                    {
                        Name = "Тетрадь в линейку 48 листов",
                        Description = "Тетрадь в линейку на 48 листов. Чёткие ровные линии, белая бумага без бликов. Подходит для конспектов и дневников.",
                        Price = 140
                    },
                    new Product
                    {
                        Name = "Тетрадь в линейку 24 листа",
                        Description = "Аккуратная тетрадь в линейку на 24 листа. Удобный размер для повседневного использования. Прочная обложка с ярким дизайном.",
                        Price = 110
                    },
                    new Product
                    {
                        Name = "Тетрадь в линейку 18 листов",
                        Description = "Небольшая тетрадь в линейку на 18 листов. Лёгкая и удобная для коротких записей. Отличный выбор для студентов и школьников.",
                        Price = 95
                    },
                    new Product
                    {
                        Name = "Небольшой набор цветных карандашей",
                        Description = "Набор из 5 цветных карандашей (красный, зелёный, синий, чёрный, жёлтый). Яркие цвета, мягкое письмо. Подходит для рисования и заметок.",
                        Price = 120
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
