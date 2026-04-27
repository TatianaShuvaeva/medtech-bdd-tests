# 🏥 MedTech BDD-Testkurs — Schnelleinstieg

## 📖 Vollständiger Kurs
Umfassender BDD-Testkurs mit Reqnroll, Playwright und Blazor:
👉 **[Siehe: `Docs/bdd-course.md`](./Docs/bdd-course.md)**

Der Kurs ist strukturiert in **11 Tage** mit praktischen Übungen und Mini-Projekten.

---

## ⚡ Quick Start (5 Minuten)

### 1. NuGet-Pakete installieren
```bash
dotnet restore
```

### 2. Playwright-Browser herunterladen
```bash
pwsh bin/Debug/net8.0/playwright.ps1 install chromium
```

### 3. Tests ausführen
```bash
# Alle Tests
dotnet test

# Nur Unit-Tests (keine Browser-Tests)
dotnet test --filter "Category!=browser"

# Nur Smoke-Tests
dotnet test --filter "Category=smoke"

# Nur regulatorische Tests
dotnet test --filter "Category=regulatorisch"
```

---

## 📁 Projektstruktur

```
MedTech.Tests/
├── Features/                    # .feature-Dateien (Gherkin)
│   ├── Medikamentenverschreibung.feature
│   ├── Patientenakte.feature
│   └── Rezeptverwaltung.feature
├── StepDefinitions/             # C# Step-Implementierungen
│   ├── RezeptSteps.cs
│   ├── PatientenakteSteps.cs
│   └── ApiSteps.cs
├── Pages/                       # Blazor Page Object Model
│   ├── BasePage.cs
│   └── RezeptPage.cs
├── Hooks/                       # Setup/Teardown
│   ├── DatabaseHooks.cs
│   └── PlaywrightHooks.cs
├── Infrastructure/              # Testinfrastruktur
│   ├── TestDbContext.cs         # EF Core InMemory
│   └── RezeptService.cs         # Business-Logik
└── reqnroll.json                # Reqnroll-Konfiguration (Deutsch)
```

---

## 🔑 Wichtige Konzepte

### ✅ Test-Pyramide
- **🟢 70% Unit-Tests** — Business-Logik (RezeptService)
- **🟡 20% Integrationstests** — API + InMemory DB
- **🔴 10% E2E-Tests** — Blazor UI mit Playwright

### ✅ Feature-Sprache: Deutsch
Mit `reqnroll.json` (`language: de-DE`) sind Schlüsselwörter auf Deutsch:
- `Funktionalität:` statt `Feature:`
- `Szenario:` statt `Scenario:`
- `Angenommen` / `Wenn` / `Dann` statt `Given` / `When` / `Then`

### ✅ InMemory-DB Isolation
Jedes Szenario bekommt eine neue Datenbank (mit eindeutiger GUID):
```csharp
var dbName = $"MedTechTest_{Guid.NewGuid()}";
var options = new DbContextOptionsBuilder<TestDbContext>()
    .UseInMemoryDatabase(dbName)  // ← Isolation!
    .Options;
```

### ✅ Tags für CI/CD-Filterung
```gherkin
@smoke          # Schnelle Smoke-Tests (< 5 Sekunden)
@sicherheit     # Sicherheitskritisch (Allergie, Wechselwirkungen)
@regulatorisch  # MDR/Audit-relevant (Compliance)
@browser        # Playwright-Tests (langsam, nur bei Bedarf)
@api            # REST-API-Tests
```

---

## 🧪 Test schreiben — Workflow

### 1. Feature-File schreiben (`.feature`)
```gherkin
# Features/Medikamentenverschreibung.feature
@sicherheit @smoke
Scenario: Allergie-Warnung bei Penicillin
  Given patient "Anna Klein" has allergy to "Penicillin"
  When doctor prescribes "Amoxicillin"
  Then allergy warning should appear
  And prescription should NOT be saved
```

### 2. Steps implementieren (`.cs`)
```csharp
// StepDefinitions/RezeptSteps.cs
[Binding]
public class RezeptSteps
{
    [Given("patient {string} has allergy to {string}")]
    public void PatientHatAllergie(string name, string allergen)
    {
        var patient = new Patient { Name = name };
        patient.Allergien.Add(allergen);
        _db.Patienten.Add(patient);
        _db.SaveChanges();
    }

    [When("doctor prescribes {string}")]
    public void ArztVerschreibt(string medikament)
    {
        var result = _rezeptService.VerschreibeMedikament(_patient, medikament);
        _context["Ergebnis"] = result;
    }

    [Then("allergy warning should appear")]
    public void WarnungErscheint()
    {
        var result = _context.Get<RezeptErgebnis>("Ergebnis");
        result.Warnung.Should().NotBeNull();
    }
}
```

### 3. Test ausführen
```bash
dotnet test --filter "Scenario=Allergie-Warnung bei Penicillin"
```

---

## 🎯 Häufige Fehler vermeiden

### ❌ NICHT: UI-Implementierungsdetails in Features
```gherkin
# ❌ Falsch
When I click button with XPath "//button[@id='prescribe-btn']"
```

### ✅ JA: Fachliches Verhalten beschreiben
```gherkin
# ✅ Richtig
When the doctor prescribes "Amoxicillin"
Then an allergy warning should appear
```

### ❌ NICHT: Alle Tests in E2E-Browser-Tests
```csharp
// ❌ Falsch: 30 Sekunden pro Test
[Test]
public async Task Allergie_Warnung()
{
    await browser.GotoAsync("https://...");  // 2s
    await browser.FillAsync("#patient", "Anna"); // 2s
    // ... 50 weitere Zeilen Browser-Automatisierung
}
```

### ✅ JA: Hauptlogik in Unit-Tests, nur UI-Verhalten in E2E
```csharp
// ✅ Richtig: 10ms
[Test]
public void Allergie_Warnung()
{
    var result = service.VerschreibeMedikament(patient, "Amoxicillin");
    result.Warnung.Should().NotBeNull();
}

// Und nur 1 E2E-Test: "Warnung erscheint rot auf der Seite"
[Test]
public async Task Allergie_Warnung_UI()
{
    await page.Locator("[data-testid='allergy-warning']").IsVisibleAsync();
}
```

---

## 🔧 Nützliche CLI-Befehle

```bash
# Build + Test
dotnet build
dotnet test

# Nur spezifische Tags
dotnet test --filter "Category=smoke"
dotnet test --filter "Category!=browser"  # Alles außer Browser

# Mit detaillierten Logs
dotnet test --logger "console;verbosity=detailed"

# Test-Results anschauen
dotnet test --logger "trx" --results-directory TestResults

# Playwright-Tests headful (sichtbar) laufen lassen
set CI=false
dotnet test --filter "Category=browser"
```

---

## 📚 Weiterführende Ressourcen

| Thema | Link |
|-------|------|
| **Vollständiger Kurs** | [Docs/bdd-course.md](./Docs/bdd-course.md) |
| Reqnroll Dokumentation | https://docs.reqnroll.net |
| Playwright .NET | https://playwright.dev/dotnet |
| EF Core InMemory | https://learn.microsoft.com/ef/core/providers/in-memory |
| Gherkin Syntax | https://cucumber.io/docs/gherkin/ |

---

## 🆘 Support

Bei Fragen: Siehe **[`Docs/bdd-course.md`](./Docs/bdd-course.md)** — dort sind alle 11 Tage mit Beispielen und Übungen ausführlich dokumentiert.

Viel Erfolg! 🚀🏥