Feature: CreateBooking
	In order to make a reservation
	As a user
	I want to create a booking for a room

	Background: 
		Given the fully occupied range is from "2023-12-01" to "2023-12-05"
		
Scenario: 1. Book room when StartDate and EndDate is before fully occupied range
	When I request to create a booking with SD "2023-11-28" and ED "2023-11-30"
	Then the booking should be successful
	
Scenario: 2. Book room when StartDate is after the fully occupied range and EndDate is after the fully occupied range 
	When I request to create a booking with SD "2023-12-10" and ED "2023-12-15"

	Then the booking should be successful
	
Scenario: 3. Cannot book a room when StartDate is after the fully occupied range and the EndDate is before the fully occupied date
	When I request to create a booking with SD "2023-12-06" and ED "2023-11-10"
	Then the booking should fail
	
Scenario: 4. Cannot book a room when StartDate is one day before the fully occupied starts range and EndDate is the first day of the fully occupied range
	When I request to create a booking with SD "2023-11-30" and ED "2023-12-01"
	Then the booking should fail
	
Scenario: 5. Cannot book a room when StartDate is before the fully occupied range and the EndDate is the last day of the fully occupied range
	When I request to create a booking with SD "2023-11-30" and ED "2023-12-05"
	Then the booking should fail
	
Scenario: 6. Cannot book a room when StartDate is the first day of the fully occupied range and the EndDate is a day after the fully occupied range
	When I request to create a booking with SD "2023-12-01" and ED "2023-12-06"
	Then the booking should fail
	
Scenario: 7. Cannot book a room when StartDate is the last day of the fully occupied range and EndDate is a day after the fully occupied range
	When I request to create a booking with SD "2023-12-05" and ED "2023-12-9"
	Then the booking should fail
	
	