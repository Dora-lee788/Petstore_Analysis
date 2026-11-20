# Petstore API — анализ результатов тестирования

Автоматизированное тестирование REST API Petstore с использованием двух фреймворков: xUnit и NUnit.

**Тестируемый API:** [Petstore Swagger](https://petstore.swagger.io/v2)

## Содержание

1. [Количественный анализ](./Analysis1.md)
2. [Качественный анализ](./Analysis2.md)
3. [Контрольные вопросы](./ControlQuestions.md)
4. [Проект xUnit](./Petstore.XUnit/)
5. [Проект NUnit](./Petstore.NUnit/)

## Результаты тестирования

| Показатель | xUnit | NUnit |
|---|---:|---:|
| Всего тестов | 38 | 38 |
| Успешно пройдено | 29 | 29 |
| Завершилось с ошибкой | 9 | 9 |
| Время выполнения | 4,9870 сек. | 11,2591 сек. |

# NUnit:

<img width="974" height="159" alt="image" src="https://github.com/user-attachments/assets/ec0f23b6-335b-4a74-9679-4eaa1c400bea" />

# xUnit:

<img width="974" height="71" alt="image" src="https://github.com/user-attachments/assets/a07a4172-81e6-485b-8b8c-ffe32a000013" />

# Тесты суммарно вместе: 

<img width="974" height="57" alt="image" src="https://github.com/user-attachments/assets/9cd69d11-49f4-4569-9ebe-6bcc4819d7d8" />


## Запуск тестов

Из корневой папки проекта выполнить команду:

```powershell
dotnet test .\Petstore_Analysis.sln
