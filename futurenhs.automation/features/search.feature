Feature: Search
    Feature covering search functionality of FNHS

# Lots of questions unanswered, unable to script journeys without further clarification on process. 

Background:     
    Given I have navigated to '/'
    And I have logged in as a 'user' and accept the cookies
    Given I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed

Scenario Outline: FNHS97 - Search is available validation
    Given I have navigated to '<url>'
    Then the search bar is available
Examples:
    | url                                                    |
    | /                                                      |
    | groups/                                                |
    | groups/aa/                                             |
    | groups/aa/members                                      |
    | groups/aa/members/d74ed860-9ea5-4c95-9394-ad3a00924fa5 |
    | groups/aa/folders                                      |
    | groups/aa/folders/create                               |
    | groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb |
    | groups/aa/files/54249d0f-3544-4b76-b6fb-ae0001022c09   |
    | groups/aa/forum                                        |
    | groups/aa/forum/create                                 |
    | groups/aa/forum/3fbc0d4e-c2a7-46a0-99e9-ad7f010eb4ad   |
    | groups/discover                                        |
    | /terms-and-conditions                                  |
    | /privacy-policy                                        |


Scenario: FNHS98 - Search results display validation
    When I search for 'Automation'
    Then the 'Searching: Automation - 6 results found' header is displayed
    And there are '6' search results displayed
    And the 'Automation Admin Group' search result card is displayed
    | Group                                                                            |
    | DO NOT USE - This group is reserved solely for use by our automated test scripts |


Scenario: FNHS99 - Search for a Group by Group Title
    When I search for 'Automation Admin'
    Then the 'Searching: Automation Admin - 1 results found' header is displayed
    And the 'Automation Admin Group' search result card is displayed
    | Group                                                                            |
    | DO NOT USE - This group is reserved solely for use by our automated test scripts |
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed

Scenario: FNHS100 - Search for a Group That Doesn't Exist
    When I search for 'Automation Group'
    Then the 'Searching: Automation Group - 0 results found' header is displayed
    And the 'Sorry no results found' textual value is displayed


Scenario: FNHS101 - Search for a Group Where Not a Member
    When I search for 'Digital Primary'
    Then the 'Searching: Digital Primary - 1 results found' header is displayed
    And the 'Digital Primary Care' search result card is displayed
    | Group                                                                                         |
    | Support colleagues to share experiences, learning and resources on the digital transformation |
    When I click the 'Digital Primary Care' link
    Then the 'Digital Primary Care' header is displayed


Scenario: FNHS102 - Search for a Discussion by the Title
    When I search for 'General Discussion Validation'
    Then the 'Searching: Discussion - 1 results found' header is displayed
    And the 'General Discussion Validation' search result card is displayed
    # | Discussion on Automation Admin Group group forum | 
    When I click the 'General Discussion Validation' link
    Then the 'General Discussion Validation' header is displayed


Scenario: FNHS103 - Search for a Comment by Comment Text
    When I search for 'Comment for Like test'
    Then the 'Searching: Comment for Like test - 2 results found' header is displayed
    And the 'Comment on discussion: General Discussion Validation' search result card is displayed
    | Discusson on Automation Admin Group group forum |
    | Comment for Like test                           |
    And the 'General Discussion Validation' search result card is displayed
    | Discusson on Automation Admin Group group forum |
    | Comment for Like test                           |
    When I click the 'Comment on discussion: General Discussion Validation' link
    Then the 'Comment for Like test' comment card is displayed


Scenario: FNHS104 - Search for a File by File Name
    When I search for 'docTest'
    Then the 'Searching: docTest - 2 results found' header is displayed
    And the 'DocTest' search result card is displayed
    | File on Automation Admin Group group |
    | Test doc                             |
    And the 'DocTest' search result card is displayed
    | File on Automation Public Group group |
    | Test doc                              |
    When I click the 'DocTest' link
    # FILE PAGE VALIDATION
    # Then the 'DocTest' comment card is displayed


Scenario: FNHS105 - Search for a File by File Description
    When I search for 'Test doc'
    Then the 'Searching: docTest - 3 results found' header is displayed
    And the 'DocTest' search result card is displayed
    | File on Automation Admin Group group |
    | Test doc                             |
    And the 'DocTest' search result card is displayed
    | File on Automation Public Group group |
    | Test doc                              |
    And the 'Test doc' search result card is displayed
    | File on Automation Visual Regression Group group |
    | Test doc                                         |
    When I click the 'Test doc' link
    # FILE PAGE VALIDATION
    # Then the 'Test doc' comment card is displayed


Scenario: FNHS106 - Search for a Folder by Folder Name
    When I search for 'Automation Test Folder'
    Then the 'Searching: Automation Test Folder - 1 results found' header is displayed
    And the 'Automation Test Folder' search result card is displayed
    | Folder on Automation Admin Group group | 
    When I click the 'Automation Test Folder' link


Scenario: FNHS107 - Search for a File by Folder Description
    When I search for 'Empty folder for testing'
    Then the 'Searching: Empty folder for testing - 2 results found' header is displayed
    And the 'Public Empty Folder' search result card is displayed
    | Folder on Automation Public Group group |
    | Empty Folder for testing                |
    And the 'Empty Folder' search result card is displayed
    | Folder on Automation Admin Group group |
    | Empty Folder for testing               |
    When I click the 'Empty Folder' link
    Then the 'Empty Folder' header is displayed


@Pending
Scenario: FNHS108 - Search for a Group By the Strapline