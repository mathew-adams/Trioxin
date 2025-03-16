# **Trioxin (two-four-five)**
An **expression-based rule engine** for model validation, value calculation and model state management in **Blazor** applications.

## **Overview**
Trioxin is a **powerful, declarative validation and calculation engine** designed to work seamlessly with **MudBlazor forms**. By leveraging **expression-based rules**, Trioxin dynamically controls field visibility, enables/disables inputs, and performs real-time calculations—all without requiring manual event handling.

With **Trioxin**, you can:
- **Define validation rules** directly in model properties.
- **Calculate dependent field values** using expressions.
- **Control visibility and enable/disable fields** based on dynamic conditions.
- **Enforce business logic** without writing additional UI logic.

---

## **How Trioxin Works**
Trioxin operates by attaching **rules** to model properties using the `[TrioxinRule]` attribute. These rules define how a field behaves based on other field values.

### **Supported Rule Types**
| Rule Type    | Description |
|-------------|------------|
| **Required**  | Makes a field mandatory based on a condition. |
| **Enabled**   | Controls whether a field is enabled or disabled. |
| **Visible**   | Dynamically shows or hides a field. |
| **Calculation** | Computes a field value based on an expression. |

---

## **Example: Mortgage Calculation Using Trioxin**
The following **`Mortgage` model** demonstrates how Trioxin manages required fields, visibility, and calculations.

### **C# Model Class**
```csharp
using Trioxin;
namespace MudBlazorWebApp1.Models;

public class Mortgage
{
    [TrioxinRule(Type = Rule.Required, Rule = "PurchasePrice = 0")]
    public decimal PurchasePrice { get; set; }

    [TrioxinRule(Type = Rule.Enabled, Rule = "false")]
    public decimal TransferTaxRate { get; set; } = 0.2M;

    [TrioxinRule(Type = Rule.Calculation, Rule = "PurchasePrice * TransferTaxRate")]
    public decimal TransferTax { get; set; }

    [TrioxinRule(Type = Rule.Visible, Rule = "PurchasePrice != 0")]
    [TrioxinRule(Type = Rule.Calculation, Rule = "PurchasePrice + TransferTax")]
    public decimal Total { get; set; }
}
```

### **Blazor Component (Razor)**
```razor
@using Models;
@using Trioxin;

@page "/"

<PageTitle>Home</PageTitle>

<MudGrid>
    <MudItem xs="12" sm="7">
        <MudPaper Class="pa-4">
            <!-- 
            ==============================================
            | MUD BLAZOR FORM VALIDATION                 |
            | A MudBlazor Form can utilize a validation  |
            | function to invoke the Trioxin rules       |
            ==============================================
            -->
            <MudForm Model="@Mortgage" @ref="form" Validation="ValidateValue">

                <!-- 
                  ==============================================
                  | MUD BLAZOR TEXT FIELD PROPERTIES           |
                  | MudBlazor text fields can utilize the      |
                  | calculation indexor for the following:     |
                  |  - Required                                |
                  |  - Disabled                                |
                  |  - ReadOnly                                |
                  ==============================================
                -->
                <MudTextField @bind-Value="Mortgage.PurchasePrice"
                    Required="Calc[nameof(Mortgage.PurchasePrice)].Required"
                    Disabled="!Calc[nameof(Mortgage.PurchasePrice)].Enabled"
                    For="@(()=>Mortgage.PurchasePrice)"
                    Variant="Variant.Outlined"
                    Label="PurchasePrice" />

                <MudTextField @bind-Value="Mortgage.TransferTaxRate"
                    For="@(()=>Mortgage.TransferTaxRate)"
                    Disabled="!Calc[nameof(Mortgage.TransferTaxRate)].Enabled"
                    Variant="Variant.Outlined"
                    Label="TransferTaxRate" />

                <MudTextField @bind-Value="Mortgage.TransferTax"
                    For="@(()=>Mortgage.TransferTax)"
                    Disabled="!Calc[nameof(Mortgage.TransferTax)].Enabled"
                    Variant="Variant.Outlined"
                    Label="TransferTax" />

                <!-- 
                  ==============================================
                  | VISIBILITY BASED ON INDEXOR PROPERTY       |
                  | For visibility, wrap the element in an     |
                  | IF statement against the indexor property  |
                  | for Visible                                |
                  ==============================================
                -->
                @if(Calc[nameof(Mortgage.Total)].Visible)
                {
                   <MudTextField @bind-Value="Mortgage.Total"
                        For="@(()=>Mortgage.Total)"
                        Disabled="!Calc[nameof(Mortgage.Total)].Enabled"
                        Variant="Variant.Outlined"
                        Label="Total" />
                }

            </MudForm>
            <MudCheckBox Value="@form.IsValid">Is valid</MudCheckBox>
        </MudPaper>
        <MudPaper Class="pa-4 mt-4">
            <MudButton Variant="Variant.Filled" Color="Color.Primary" DropShadow="false" OnClick="@(()=>form.Validate())">Validate</MudButton>
            <MudButton Variant="Variant.Filled" Color="Color.Secondary" DropShadow="false" OnClick="@(()=>form.ResetAsync())" Class="mx-2">Reset</MudButton>
            <MudButton Variant="Variant.Filled" DropShadow="false" OnClick="@(()=>form.ResetValidation())">Reset Validation</MudButton>
        </MudPaper>
    </MudItem>
</MudGrid>
```
### **C# Code (Blazor @code Block)**
```razor
@code {
    MudForm form = new();
    Mortgage Mortgage = new();
    ModelValidator<Mortgage> Calc;

    protected override void OnInitialized()
    {
        // ==============================================
        // INITIALIZING MODEL VALIDATOR
        // Initialize the ModelValidator against the model.
        // Optionally, you can provide a function to retrieve 
        // additional values by name.
        // ==============================================
        Calc = new ModelValidator<Mortgage>((name) =>
        {
            if (name == "Mode") return 1;
            return false;
        });

        // ==============================================
        // RUN INITIAL VISIBILITY & ENABLED RULES
        // Run validation for initial Visibility and Enabled rules
        // ==============================================
        Calc.Validate(ref Mortgage);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, field) =>
    {
        // ==============================================
        // MODEL VALIDATION FUNCTION
        // Pass the model into the Validation function.
        // The model is by reference, meaning values will be 
        // updated based on Calculation rule types.
        //
        // Message keys will return for any Required or Errors.
        // ==============================================
        Mortgage context = (Mortgage)model;
        Calc.Validate(ref context);
        return Calc[field].MessageKeys;
    };
}
```

## Operations and functions
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
