DECLARE @Sql nvarchar(max) = N'SELECT x=''One\' + CHAR(13) + CHAR(10)
    + 'Two\' + CHAR(10)
    + 'Three\' + CHAR(13)
    + 'Four'' FOR JSON PATH'
EXEC (@Sql);
-- yields N'[{"x":"OneTwoThree\rFour"}]'

-- Order is important! We do not want to replace our replacements
SELECT @Sql = REPLACE(@Sql, '\'+CHAR(10),
    '\\'+CHAR(10)+CHAR(10))
SELECT @Sql = REPLACE(@Sql, '\'+CHAR(13)+CHAR(10),
    '\\'+CHAR(10)+CHAR(13)+CHAR(10))
SELECT @Sql = REPLACE(@Sql, '\'+CHAR(13),
    '\\'+CHAR(13))
EXEC (@Sql);
-- yields N'[{"x":"One\\\r\nTwo\\\nThree\\\rFour"}]'