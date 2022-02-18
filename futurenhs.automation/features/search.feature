@Pending
Feature: Search
    Feature covering search functionality of FNHS

# Lots of questions unanswered, unable to script journeys without further clarification on process. 

Background:     
    Given I have navigated to '/'
    And I have logged in as a 'user'
    Then the 'My Groups' header is displayed

Scenario: FNHSXX - Search Results different results card validation
Scenario: FNHSXX - Search for a Group
Scenario: FNHSXX - Search for a Group That Doesn't Exist
Scenario: FNHSXX - Search for a Group Where Not a Member
Scenario: FNHSXX - Search for a Group By the Strapline
Scenario: FNHSXX - Search for a Discussion
Scenario: FNHSXX - Search for a Discussion Using inital 
Scenario: FNHSXX - Search for a Comment
Scenario Outline: FNHSXX - Search for a File
Examples:
    | search input    |
    | File Title/Name |
    | File Descripton |
Scenario Outline: FNHSXX - Search for a Folder
Examples:
    | search input      |
    | Folder Name       |
    | Folder Descripton |