﻿using Exportable.Attribute;
using Exportable.Models;
using Workshop.Infrastructure.ExcelHandler;

namespace Workshop.Model.Excel
{
    public class ProductInfoExcel
    {
        [ExcelImport("批次", order = 1)]
        [Exportable(0,"批次",FieldValueType.Text)]
        public string Title { get; set; }
        [ExcelImport("编号", order = 2)]
        [Exportable(1,"编号",FieldValueType.Text)]
        public string ProductNumber { get; set; }
    }
}