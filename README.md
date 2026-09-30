# Reviews Assistant

**Reviews Assistant** — навчальний full-stack застосунок із вебінару про інтеграцію OpenAI API з .NET. Користувачі залишають відгуки, а адміністратор за допомогою AI аналізує їхню тональність, пріоритет, категорію й терміновість, а також генерує чернетки відповідей.

Технології: ASP.NET Core (.NET 10), Angular, PostgreSQL, Entity Framework Core, JWT та OpenAI Responses API.

## Передумови

- .NET SDK 10;
- Node.js і npm;
- PostgreSQL;
- OpenAI API key.

## Налаштування

Відкрийте `api/ReviewsAssistant/ReviewsAssistant.WebAPI/appsettings.json` і замініть значення-заглушки:

- `ConnectionStrings:Reviews` — рядок підключення до PostgreSQL;
- `Jwt:SigningKey` — довгий випадковий ключ для підпису JWT;
- `Admin:Email` та `Admin:Password` — облікові дані адміністратора;
- `Ai:OpenAi:ApiKey` — ваш ключ OpenAI API.

Щоб увімкнути AI-аналіз, `Ai:Provider` має мати значення `OpenAi`. Для роботи без викликів OpenAI змініть його на `Stub`.

> Не публікуйте реальні ключі або паролі. Для локальної розробки зручніше зберігати їх у `appsettings.Development.json` або User Secrets — файл `appsettings.Development.json` ігнорується Git.

Приклад секції AI:

```json
"Ai": {
  "Provider": "OpenAi",
  "OpenAi": {
    "Model": "gpt-5-mini",
    "ApiKey": "YOUR_OPENAI_API_KEY"
  }
}
```

## Запуск

1. Створіть базу даних PostgreSQL і налаштуйте рядок підключення.
2. Запустіть сервер в одному терміналі:

   ```bash
   cd api/ReviewsAssistant/ReviewsAssistant.WebAPI
   dotnet run
   ```

   Під час запуску застосунок автоматично застосує міграції. API буде доступне за адресою `http://localhost:5296`.

3. В іншому терміналі встановіть залежності та запустіть клієнт:

   ```bash
   cd client
   npm install
   npm start
   ```

4. Відкрийте адресу, яку виведе Angular CLI. За замовчуванням клієнт звертається до API за адресою `http://localhost:5296`.

## Можливості

- публічна форма для надсилання відгуків;
- JWT-вхід до адміністративної панелі;
- фільтрація та сортування відгуків;
- AI-аналіз тональності, категорії, пріоритету й терміновості;
- створення чернетки відповіді клієнту.
