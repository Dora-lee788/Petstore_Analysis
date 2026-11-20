# Petstore API Testing — xUnit & NUnit

Тестирование REST API Petstore с использованием фреймворков xUnit и NUnit.

**API:** https://petstore.swagger.io/v2

## Содержание

- [Количественный анализ](Analysis_1.md)
- [Качественный анализ](Analysis_2.md)
- [Контрольные вопросы](Control_Questions.md)
- [Тесты xUnit](Petstore.XUnit/)
- [Тесты NUnit](Petstore.NUnit/)

## Результаты тестирования

| Показатель | xUnit | NUnit |
|---|---:|---:|
| Всего тестов | 38 | 38 |
| Успешно | 29 | 29 |
| С ошибкой | 9 | 9 |
| Время выполнения | 4,9870 сек. | 11,2591 сек. |

## Запуск

```powershell
dotnet test .\Petstore_Analysis.sln --logger "console;verbosity=normal"


