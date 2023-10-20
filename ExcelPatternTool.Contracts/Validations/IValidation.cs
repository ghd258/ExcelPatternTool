﻿using ExcelPatternTool.Contracts.Patterns;

namespace ExcelPatternTool.Contracts.Validations
{
    public interface IValidation
    {
        Convention Convention { get; set; }
        string Description { get; set; }
        string Expression { get; set; }
        ProcessResult ProcessResult { get; set; }
        Target Target { get; set; }
    }
}