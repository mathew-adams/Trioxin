# Trioxin (2-4-5)

An expression calculor for validation of models

| Operator | Description  | Usage                  | Output | Supports multiple arguments | Notes |
|----------|-------------|------------------------|--------|-----------------------------|-------|
| +        | Add         | 1 + 2                  | 3      |                             |       |
| -        | Minus       | 2 - 1                  | 1      |                             |       |
| *        | Multiply    | 2 * 2                  | 4      |                             |       |
| /        | Divide      | 4 / 2                  | 2      |                             |       |
| >        | Greater than | 3>2                    | True   |                             |       |
| <        | Less than   | 2<3                    | True   |                             |       |
| <=       | Less than or equal to | 3<=3       | True   |                             |       |
| >=       | Greater than or equal to | 4>=3    | True   |                             |       |
| =        | Check if two strings are exactly equal, case sensitive | "Hello"="Hello" | True | | |
| ~=       | Check if two strings are equal, case insensitive | "hello"="HeLLO" | True | | |
| IF       | Evaluate a condition and return either true or false | IF(expression, true_result, false_result) | True result or false result | | |
| OR       | Check if any of two or more expressions is true | OR(true, false) | True | Yes | |
| IN       | Check if a value is contained within a list | IN(2, 3, 4, 5, 6) | True | Yes | |
| AND      | Check if two or more expressions are true | AND(true, true) | True | Yes | |
| NOT      | Return the opposite boolean | NOT(true) | False | | |
| MIN      | Get the minimum value of a range of numbers | MIN(1, 2, 3, 4, 5, 6) | 1 | Yes | The first position is the search term |
| MAX      | Get the maximum value of a range of numbers | MAX(1, 2, 3, 4, 5, 6) | 6 | Yes | The first position is the search term |
| SUM      | Get the sum of a range of numbers | SUM(1, 2, 3, 4, 5, 6) | 21 | Yes | |
| AVG      | Get the average of a range of numbers | AVG(1, 2, 3, 4, 5, 6) | 3.5 | Yes | |
| ABS      | Get the absolute value of a number | ABS(-2) | 2 | | |
| LEN      | Get the length of a string | LEN("hello world") | 11 | | |
| LEFT     | Take X number of characters from the left | LEFT("hello world", 3) | hel | | |
| CINT     | Convert to integer | CINT(2) | <int>2 | | |
| CBOOL    | Convert to boolean | CBOOL(1) | <bool>true | | |
| CBYTE    | Convert to byte | CBYTE(2) | <byte>2 | | |
| CLONG    | Convert to long | CLONG(200000000) | <long>200000000 | | |
| CSHORT   | Convert to short | CSHORT(200) | <short>200 | | |
| RIGHT    | Take X number of characters from the right | RIGHT("hello world", 3) | rld | | |
| ROUND    | Round a number to X digits | ROUND(123.43545454545, 2) | 123.44 | | |
| WITHIN   | Check if a number is within a range, inclusive of start and end | WITHIN(2, 3, 6) | True | The first position is the search term |
| BETWEEN  | Check if a number is between a range, exclusive of start and end | BETWEEN(2, 3, 6) | False | The first position is the search term |
