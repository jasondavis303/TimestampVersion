﻿using System;

namespace TimestampVersion
{
    public static class Generator
    {
        public static Version Generate(DateTime dateTime) => new Version(dateTime.Year - 2000, dateTime.Month, dateTime.Day, (dateTime.Hour * 60) + dateTime.Minute);

        public static Version Generate() => Generate(DateTime.UtcNow);
    }
}