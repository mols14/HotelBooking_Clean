Feature: CreateBooking
	In order to make a reservation
	As a user
	I want to create a booking for a room

	Background: 
		Given the fully occupied range is from "2023-12-02" to "2023-12-07"
		
Scenario: 1. Book room when StartDate and EndDate is after fully occupied range
	When I request to create a booking with SD "2023-12-08" and ED "2023-12-09"
	Then the booking should be successful
	
Scenario: 2. Book room when StartDate is after the fully occupied range and EndDate is after the fully occupied range 
	When I request to create a booking with SD "2023-11-04" and ED "2023-11-07"
	Then the booking should be successful
	
Scenario: 3. Cannot book a room when StartDate is before the fully occupied start range and the EndDate is before the fully occupied end date
	When I request to create a booking with SD "2023-12-01" and ED "2023-12-05"
	Then the booking should fail
	
Scenario: 4. Cannot book a room when StartDate is before the fully occupied start range and EndDate is before the fully occupied end date
	When I request to create a booking with SD "2023-11-03" and ED "2023-12-04"
	Then the booking should fail
	
Scenario: 5. Cannot book a room when StartDate is before the fully occupied start range and the EndDate is after the fully occupied end range
	When I request to create a booking with SD "2023-12-01" and ED "2023-12-09"
	Then the booking should fail
	
Scenario: 6. Cannot book a room when StartDate is after the fully occupied start range and the EndDate is after the fully occupied range
	When I request to create a booking with SD "2023-12-03" and ED "2024-01-01"
	Then the booking should fail
	
Scenario: 7. Cannot book a room when StartDate and enddate is within the fully occupied range
	When I request to create a booking with SD "2023-12-03" and ED "2023-12-5"
	Then the booking should fail
	
	