# MedTech — UI & Test Quickstart

## UI starten (Blazor Demo-App)

```powershell
cd MedTech.App
# Startet die Blazor UI auf http://localhost:3000
 dotnet run
```

Die App ist dann im Browser erreichbar unter: [http://localhost:3000](http://localhost:3000)

---

## Playwright UI-Tests ausführen

```powershell
cd MedTech.Tests
# Führt alle Browser-Tests (Playwright) aus
 dotnet test MedTech.Tests.csproj --filter "Category=browser"
```

Hinweis: Die Tests starten die Demo-App automatisch, falls sie noch nicht läuft.

---

## Weitere nützliche Kommandos

- **Nur Unit- und Integrationstests (ohne Browser):**
  ```powershell
  dotnet test MedTech.Tests.csproj --filter "Category!=browser"
  ```
- **Alle Tests (inkl. Browser):**
  ```powershell
  dotnet test
  ```
- **Playwright-Browser installieren (nur 1x nötig):**
  ```powershell
  pwsh bin/Debug/net8.0/playwright.ps1 install chromium
  ```
- **Test-Logs ausführlich anzeigen:**
  ```powershell
  dotnet test --logger "console;verbosity=detailed"
  ```
- **Test-Resultate als Datei:**
  ```powershell
  dotnet test --logger "trx" --results-directory TestResults
  ```

---

## Weitere Infos

- Siehe [Docs/bdd-course.md](./Docs/bdd-course.md) für den vollständigen BDD-Kurs und Details zu Tag 6 (Playwright + Blazor UI-Tests).
- Bei Problemen: Prüfe, ob Port 3000 frei ist und keine andere App blockiert.
- Die Testdaten und UI-Logik findest du in `MedTech.App/Services/MedTechDemoService.cs`.
