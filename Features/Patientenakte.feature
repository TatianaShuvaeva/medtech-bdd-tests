Feature: Patientenakte
	As a physician
	I want to review blood pressure trends
	So that I can identify hypertension risks early

	@smoke @patient-chart
	Scenario: Doctor views patient's blood pressure history
		Given patient "Klaus Bauer" has blood pressure readings:
			| Date       | Systolic | Diastolic |
			| 2024-01-10 | 145      | 92        |
			| 2024-01-17 | 138      | 88        |
			| 2024-01-24 | 150      | 95        |
		When the doctor opens the blood pressure chart
		Then the average systolic should be 144
		And the trend should be marked as "RISING"
		And a clinical alert should be shown: "Hypertension Stage 1"
