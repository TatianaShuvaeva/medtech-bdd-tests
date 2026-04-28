@medikation @sicherheit @regression
Feature: Medikamentenverschreibung
  Als behandelnder Arzt
  Möchte ich Medikamente sicher verschreiben
  Damit Patienten die richtige Behandlung ohne schädliche Wechselwirkungen erhalten

  Background:
    Given Dr. Weber ist im System angemeldet
    And Patient "Maria Hoffmann" (ID: "P-4421") ist geöffnet

  @audit-relevant @smoke @kritisch @positive
  Scenario: Erfolgreiche Medikamentenverschreibung
    When Dr. Weber "Ibuprofen 400mg" zweimal täglich verschreibt
    Then sollte das Rezept gespeichert werden
    And das Medikament sollte in den aktiven Medikamenten des Patienten erscheinen
    And eine Rezept-PDF sollte generiert werden

  @sicherheit @allergie @negativ
  Scenario: Warnung bei bekannter Patientenallergie
    Given Patient "Maria Hoffmann" hat eine dokumentierte Allergie gegen "Penicillin"
    When Dr. Weber versucht "Amoxicillin" zu verschreiben
    Then sollte eine rote Allergie-Warnung erscheinen: "Patient ist allergisch gegen Penicillin-Klasse-Antibiotika"
    And das Rezept sollte NICHT automatisch gespeichert werden
    And Dr. Weber muss die Überschreibung mit einem Grund bestätigen

  @sicherheit @wechselwirkung
  Scenario: Warnung bei Medikamentenwechselwirkung
    Given Patient "Maria Hoffmann" nimmt aktuell "Warfarin" ein
    When Dr. Weber "Aspirin 100mg" verschreibt
    Then sollte eine Wechselwirkungs-Warnung erscheinen: "Erhöhtes Blutungsrisiko mit Warfarin"
    And der Schweregrad sollte als "HOCH" markiert sein
    And das System sollte "Alternative in Betracht ziehen: Paracetamol" vorschlagen