API טופס תלונות ציבוריות - Public Complaint Form API
תוכן עניינים
תיאור השרת

מבנה הפרויקט

שיטת הבנייה

הרכיבים הראשיים

אבטחה

טיפול בשגיאות

קרוס דומיין (CORS)

הוראות התקנה והפעלה

Endpoints ו-API

תיאור השרת
Public Complaint Form API הוא שרת Backend מבוסס .NET 8 המהווה את ליבת המערכת להגשת תלונות ציבוריות לבתי המשפט. השרת מנהל את כל מחזור החיים של הפנייה: החל מאימות בוטים (Captcha), דרך ניקוי נתונים (Sanitization), שמירה מאובטחת במסד נתונים SQL Server, ועד להפקת דוחות סטטיסטיים השוואתיים.

מבנה הפרויקט
הפרויקט בנוי בארכיטקטורת שכבות (Services & Models) להפרדה מקסימלית בין לוגיקה עסקית לגישה לנתונים:

PublicComplaintForm_API/
├── Services/                # לוגיקה עסקית ושירותים
│   ├── DatabaseService.cs   # ניהול SQL (Dapper) ודוחות
│   ├── CaptchaService.cs    # יצירת תמונות אימות (ImageSharp)
│   ├── SanitizingService.cs # הגנה מפני XSS (HtmlSanitizer)
│   └── LogService.cs        # רישום אירועים (log4net)
├── Models/                  # אובייקטי נתונים (DTOs)
│   ├── PublicComplaintData.cs
│   ├── MonthlyReportRow.cs  # מודל לדוחות השוואתיים
│   └── Court.cs
├── Controllers/             # ניהול נקודות קצה (Endpoints)
├── Program.cs               # הגדרות Middleware, DI ו-CORS
└── log4net.config           # קונפיגורציית לוגים
שיטת הבנייה
הפרויקט עושה שימוש ב-Build Pipeline הסטנדרטי של .NET:

Restore: שחזור חבילות NuGet (Dapper, ImageSharp, HtmlSanitizer).

Build: קימפול הקוד ובדיקת תקינות טיפוסים.

Publish: אריזת הפרויקט לקובצי DLL מוכנים להרצה ב-IIS או Docker.

הרכיבים הראשיים
שירות מסד הנתונים (DatabaseService)
מבוסס על Dapper ORM לביצועים מקסימליים.

מנגנון קישור: שימוש ב-SqlConnection עם שאילתות פרמטריות למניעת הזרקות SQL.

דוחות: יישום שאילתות מורכבות הכוללות פונקציות חלון (LAG) להשוואת נתוני תלונות בין חודשים ושנים.

שירות אימות (CaptchaService)
מייצר קוד אקראי בן 6 תווים והופך אותו לתמונה (Base64) באמצעות SixLabors.ImageSharp. הקוד נשמר ב-IMemoryCache עם תוקף מוגבל לצורך אימות ב-Server-side.

אבטחה
אבטחת המידע עומדת בראש סדרי העדיפויות של הפרויקט:

Input Sanitization: כל שדה טקסט חופשי עובר דרך ה-SanitizingService המשתמש ב-HtmlSanitizer. פעולה זו מסירה תגיות HTML מסוכנות ומונעת התקפות XSS.

SQL Injection: אין שרשור מחרוזות בשאילתות. כל הפרמטרים מועברים דרך Dapper בצורה מאובטחת.

הגנה מפני בוטים: מנגנון ה-Captcha חוסם הגשות אוטומטיות.

Secrets Management: מחרוזות התחברות (Connection Strings) אינן נשמרות בקוד אלא ב-appsettings.json ובמשתני סביבה.

Security Headers: יישום HSTS ו-HTTPS Redirection בסביבת הייצור.

טיפול בשגיאות
המערכת כוללת מנגנון טיפול בשגיאות ריכוזי:

Global Exception Middleware: תופס כל חריגה (Exception) שלא טופלה ומחזיר ללקוח JSON עקבי עם קוד שגיאה, מבלי לחשוף את ה-Stack Trace.

Logging: שימוש ב-log4net לרישום שגיאות בקובץ error.log כולל רמת חומרה (Error, Warn, Info).

HTTP Status Codes: החזרה מדויקת של קודים (400 לקלט לא תקין, 404 למשאב חסר, 500 לשגיאות שרת).

קרוס דומיין (CORS)
כדי לאפשר לפרונט-אנד (Angular) לתקשר עם ה-API, הוגדרה מדיניות CORS ב-Program.cs:

הגבלת Origins: ה-API מאשר בקשות רק מהדומיינים המורשים (למשל http://localhost:4200).

Methods: אישור פעולות GET, POST, OPTIONS.

Preflight: טיפול אוטומטי בבקשות OPTIONS שהדפדפן שולח לפני ביצוע POST.

הוראות התקנה והפעלה
דרישות קדם
.NET 8 SDK

SQL Server 2019+

סביבת פיתוח (VS 2022 / VS Code)

התקנה
שכפול הפרויקט: git clone [URL]

שחזור חבילות: dotnet restore

עדכון מחרוזת התחברות ב-appsettings.json.

הפעלה
Bash

# הרצה במצב פיתוח
dotnet run --environment Development

# בנייה לייצור
dotnet publish -c Release -o ./publish
Endpoints ו-API
השרת מספק את נקודות הקצה הבאות:

Complaints: הגשת תלונה חדשה ואימות נתונים.

Courts: שליפת רשימת בתי משפט מעודכנת.

Captcha: יצירת אתגר אימות חדש.

Reports: הפקת דוחות חודשיים הכוללים השוואה לאשתקד (JSON format).