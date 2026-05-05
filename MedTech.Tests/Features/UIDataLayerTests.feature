@browser @datalayer
Feature: UI → Data Layer Verifikation
  Als Tester
  möchte ich sicherstellen, dass Benutzeraktionen in der UI korrekt im System persistiert werden
  damit UI-Tests und Backend-Tests gemeinsam die vollständige Datenpipeline abdecken

  @browser @smoke @datalayer
  Scenario: Rezepteintrag erscheint nach UI-Verschreibung im API-Endpunkt
    Given Dr. Müller ist in der Blazor-Anwendung eingeloggt
    And der API-Zustand wurde zurückgesetzt
    And Patient "Max Müller" ist in der Patientenliste ausgewählt
    When der Arzt "Ibuprofen 400mg" in der UI verschreibt
    Then eine Erfolgsmeldung sollte in der UI erscheinen
    And der API-Endpunkt "/api/rezepte" sollte ein Rezept für Patient "Max Müller" enthalten

  @browser @negativ @datalayer
  Scenario: Kein Eintrag im API-Endpunkt nach abgewiesener Verschreibung
    Given Dr. Müller ist in der Blazor-Anwendung eingeloggt
    And der API-Zustand wurde zurückgesetzt
    And Patient "Anna Klein" ist in der Patientenliste ausgewählt
    When der Arzt "Amoxicillin" in der UI verschreibt
    Then sollte eine rote Allergie-Warnung in der UI erscheinen
    And der API-Endpunkt "/api/rezepte" sollte kein Rezept für Patient "Anna Klein" enthalten
