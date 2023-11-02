using System;
using System.Windows;
using System.Windows.Controls;

namespace Calculator_ViPCS
{
    public partial class MainWindow : Window
    {
        // Переменные для хранения текущего ввода, результата, оператора и оригинального числа
        private string currentInput = string.Empty;
        private double currentResult = 0.0;
        private string currentOperator = string.Empty;
        private double originalNumber = 0.0;
        private bool isEnteringPercent = false;

        // Переменные для обработки ввода в экспоненциальной форме
        private double currentDecimalNumber = 0.0;
        private bool isEnteringExponent = false;
        private bool isFirstOperator = true;

        // Конструктор окна
        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик события при нажатии кнопки с числом
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            // Обработка ввода числа в зависимости от текущего режима (экспоненциальная форма или нет)
            if (!isEnteringExponent)
            {
                currentInput += button.Content.ToString();

                try
                {
                    originalNumber = ConvertToDecimal(currentInput);
                }
                catch (FormatException ex)
                {
                    HandleFormatException(ex);
                }
            }
            else
            {
                currentInput += button.Content.ToString();
                UpdateDisplay();
            }

            UpdateDisplay();
        }

        // Обработка исключения формата числа
        private void HandleFormatException(FormatException ex)
        {
            MessageBox.Show("Invalid number format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            currentInput = string.Empty;
            originalNumber = 0.0;
            currentResult = 0.0;
            currentOperator = string.Empty;
            isEnteringExponent = false;
            UpdateDisplay();
        }

        // Расчет процента от числа
        private double CalculatePercentage(double originalNumber, double percentValue)
        {
            switch (currentOperator)
            {
                case "+":
                    return originalNumber + (originalNumber * percentValue / 100.0);
                case "-":
                    return originalNumber - (originalNumber * percentValue / 100.0);
                case "*":
                    return originalNumber * (percentValue / 100.0);
                case "/":
                    if (percentValue != 0)
                    {
                        return originalNumber / (percentValue / 100.0);
                    }
                    else
                    {
                        MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return 0.0; // или какой-то другой смысловой результат в случае ошибки
                    }
                default:
                    MessageBox.Show("Invalid operator for percentage calculation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return 0.0; // или какой-то другой смысловой результат в случае ошибки
            }
        }

        // Обработчик события при нажатии кнопки "="
        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput) && !string.IsNullOrEmpty(currentOperator))
            {
                double operand = ConvertToDecimal(currentInput);

                switch (currentOperator)
                {
                    case "+":
                        currentResult += operand;
                        break;
                    case "-":
                        currentResult -= operand;
                        break;
                    case "*":
                        currentResult *= operand;
                        break;
                    case "/":
                        if (operand != 0)
                        {
                            currentResult /= operand;
                        }
                        else
                        {
                            MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        break;
                    case "^":
                        // Возвести в степень
                        currentResult = Math.Pow(currentResult, operand);
                        break;
                    default:
                        MessageBox.Show("Invalid operator", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                currentInput = ConvertFromDecimal(currentResult);
                currentOperator = string.Empty;
                isEnteringExponent = false;
                UpdateDisplay();
            }

            isFirstOperator = true; // Сброс флага после равно
        }

        // Обработчик события при нажатии кнопки с тригонометрической функцией
        private void TrigFunction_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string functionName = button.Content.ToString();

            double operand = ConvertToDecimal(currentInput);

            switch (functionName)
            {
                case "Sin":
                    currentResult = Math.Sin(operand);
                    break;
                case "Cos":
                    currentResult = Math.Cos(operand);
                    break;
                case "Tan":
                    currentResult = Math.Tan(operand);
                    break;
            }

            currentInput = ConvertFromDecimal(currentResult);
            UpdateDisplay();
        }

        // Обработчик события при нажатии кнопки "^" (возведение в степень)
        private void Power_Click(object sender, RoutedEventArgs e)
        {
            isEnteringExponent = true;
            currentOperator = "^";
            currentResult = ConvertToDecimal(currentInput); // Используем текущий результат как основание степени
            currentInput = string.Empty; // Стереть введенное число
        }

        // Обработчик события при нажатии кнопки "."
        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            if (!currentInput.Contains("."))
            {
                currentInput += ".";
                UpdateDisplay();
            }
        }

        // Обработчик события при нажатии кнопки с оператором (+, -, *, /)
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                if (!string.IsNullOrEmpty(currentOperator))
                {
                    CalculateResult();
                }
                else
                {
                    currentResult = ConvertToDecimal(currentInput);
                    originalNumber = currentResult; // Сохранение изначального числа
                }

                currentInput = string.Empty;

                Button button = (Button)sender;
                currentOperator = button.Content.ToString();

                if (currentOperator == "-")
                {
                    isFirstOperator = false;
                }

                isEnteringPercent = false;
            }
        }

        // Обработчик события при нажатии кнопки "%"
        private void Percent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput) && !string.IsNullOrEmpty(currentOperator))
            {
                double operand = ConvertToDecimal(currentInput);
                double result = 0.0;

                switch (currentOperator)
                {
                    case "+":
                        result = (originalNumber * operand / 100.0);
                        break;
                    case "-":
                        result = originalNumber - (originalNumber * operand / 100.0);
                        break;
                    case "*":
                        result = originalNumber * (operand / 100.0);
                        break;
                    case "/":
                        if (operand != 0)
                        {
                            result = originalNumber / (operand / 100.0);
                        }
                        else
                        {
                            MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        break;
                    default:
                        MessageBox.Show("Invalid operator for percentage calculation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                currentResult = result;
                currentInput = ConvertFromDecimal(result);
                UpdateDisplay();
            }
        }

        // Обработчик события при нажатии кнопки "!"
        private void Factorial_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                try
                {
                    int n = int.Parse(currentInput);

                    if (n < 0)
                    {
                        MessageBox.Show("Cannot calculate factorial for a negative number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int result = 1;

                    for (int i = 1; i <= n; i++)
                    {
                        result *= i;
                    }

                    currentResult = result;
                    currentInput = ConvertFromDecimal(currentResult);
                    currentOperator = string.Empty;
                    UpdateDisplay();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Invalid number format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    currentInput = string.Empty;
                    originalNumber = 0.0;
                    currentResult = 0.0;
                    currentOperator = string.Empty;
                    isEnteringExponent = false;
                    UpdateDisplay();
                }
            }
        }

        // Обработчик события при нажатии кнопки "√"
        private void SquareRoot_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                double operand = ConvertToDecimal(currentInput);
                currentResult = Math.Sqrt(operand);
                currentInput = ConvertFromDecimal(currentResult);
                currentOperator = string.Empty;
                UpdateDisplay();
            }
        }

        // Обработчик события при нажатии кнопки "Bin"
        private void DecToBin_Click(object sender, RoutedEventArgs e)
        {
            currentInput = ConvertToBinary(originalNumber.ToString());
            currentDecimalNumber = ConvertToDecimal(currentInput);
            UpdateDisplay();
        }

        // Обработчик события при нажатии кнопки "Oct"
        private void DecToOct_Click(object sender, RoutedEventArgs e)
        {
            currentInput = ConvertToOctal(originalNumber.ToString());
            currentDecimalNumber = ConvertToDecimal(currentInput);
            UpdateDisplay();
        }

        // Обработчик события при нажатии кнопки "Hex"
        private void DecToHex_Click(object sender, RoutedEventArgs e)
        {
            currentInput = ConvertToHexadecimal(originalNumber.ToString());
            currentDecimalNumber = ConvertToDecimal(currentInput);
            UpdateDisplay();
        }

        // Обработчик события при нажатии кнопки "C" (очистка)
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            currentInput = string.Empty;
            currentResult = 0.0;
            currentOperator = string.Empty;
            isEnteringExponent = false;
            originalNumber = 0.0; // Сброс сохраненного исходного числа
            UpdateDisplay();

            isFirstOperator = true; // Сброс флага после Clear
        }

        // Обработчик события при нажатии кнопки "<" (удаление последней цифры)
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                UpdateDisplay();
            }
        }

        // Обновление содержимого дисплея
        private void UpdateDisplay()
        {
            if (!isEnteringExponent)
            {
                display.Text = string.IsNullOrEmpty(currentInput)
                    ? ConvertFromDecimal(currentResult)
                    : currentInput;
            }
            else
            {
                display.Text = currentInput;
            }
        }


        // Метод CalculateResult выполняет арифметическое действие в зависимости от текущего оператора
        private void CalculateResult()
        {
            double operand = ConvertToDecimal(currentInput);

            // Выполнение операции в соответствии с текущим оператором
            switch (currentOperator)
            {
                case "+":
                    currentResult += operand;
                    break;
                case "-":
                    currentResult -= operand;
                    break;
                case "*":
                    currentResult *= operand;
                    break;
                case "/":
                    // Проверка деления на ноль
                    if (operand != 0)
                    {
                        currentResult /= operand;
                    }
                    else
                    {
                        // Вывод сообщения об ошибке в случае деления на ноль
                        MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }

            originalNumber = currentResult; // Обновление originalNumber
        }

        // Метод ConvertToDecimal преобразует строковое представление числа в десятичное значение типа double
        private double ConvertToDecimal(string input)
        {
            try
            {
                // Проверка на префиксы для шестнадцатеричной, двоичной и восьмеричной систем счисления
                if (input.StartsWith("0x"))
                {
                    return Convert.ToInt64(input, 16);
                }
                else if (input.StartsWith("0b"))
                {
                    return Convert.ToInt64(input.Substring(2), 2);
                }
                else if (input.StartsWith("0o"))
                {
                    return Convert.ToInt64(input.Substring(2), 8);
                }
                else
                {
                    // Явное указание использования точки как разделителя десятичных дробей
                    return Convert.ToDouble(input, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException)
            {
                // Обработка ошибки неверного формата числа
                MessageBox.Show("Invalid number format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                currentInput = string.Empty;
                originalNumber = 0.0;
                currentResult = 0.0;
                currentOperator = string.Empty;
                isEnteringExponent = false;
                UpdateDisplay();
                return 0.0; // или другое значение по умолчанию
            }
        }

        // Метод ConvertFromDecimal преобразует десятичное значение в строковое представление
        private string ConvertFromDecimal(double value)
        {
            return value.ToString();
        }

        // Метод ConvertToBinary преобразует десятичное число в двоичную строку
        private string ConvertToBinary(string input)
        {
            int decimalValue = Convert.ToInt32(input);
            return Convert.ToString(decimalValue, 2);
        }

        // Метод ConvertToOctal преобразует десятичное число в восьмеричную строку
        private string ConvertToOctal(string input)
        {
            int decimalValue = Convert.ToInt32(input);
            return Convert.ToString(decimalValue, 8);
        }

        // Метод ConvertToHexadecimal преобразует десятичное число в шестнадцатеричную строку с префиксом "0x"
        private string ConvertToHexadecimal(string input)
        {
            int decimalValue = Convert.ToInt32(input);
            return "0x" + Convert.ToString(decimalValue, 16).ToUpper();
        }
    }
}
