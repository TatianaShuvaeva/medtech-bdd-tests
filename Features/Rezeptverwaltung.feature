Feature: Medikamentenverschreibung
  As a treating physician
  I want to prescribe medication safely
  So that patients receive correct treatment without harmful interactions

  Background:
    Given Dr. Weber is logged into the system
    And a patient "Maria Hoffmann" (ID: "P-4421") is open

  @smoke @positive
  Scenario: Successful medication prescription
    When Dr. Weber prescribes "Ibuprofen 400mg" twice daily
    Then the prescription should be saved
    And the medication should appear in the patient's active medications
    And a prescription PDF should be generated

  @safety @allergy
  Scenario: Warning for known patient allergy
    Given patient "Maria Hoffmann" has a documented allergy to "Penicillin"
    When Dr. Weber tries to prescribe "Amoxicillin"
    Then a red allergy warning should appear: "Patient is allergic to Penicillin-class antibiotics"
    And the prescription should NOT be saved automatically
    And Dr. Weber must confirm override with a reason

  @safety @interaction
  Scenario: Drug interaction warning
    Given patient "Maria Hoffmann" is currently taking "Warfarin"
    When Dr. Weber prescribes "Aspirin 100mg"
    Then an interaction warning should appear: "Increased bleeding risk with Warfarin"
    And the severity should be marked as "HIGH"
    And the system should suggest "consider alternative: Paracetamol"