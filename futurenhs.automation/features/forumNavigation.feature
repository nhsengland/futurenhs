Feature: Forum Navigation
    User Journeys around general forum navigation as a standard member on the Future NHS platform


Background:
    Given I have navigated to '/'
    And I have logged in as a 'user' and accept the cookies
    When I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed


@Core
Scenario: FNHS49 - Load more discussions validation 
    And the 'Load more' button is displayed
    And there are discussion cards displayed
    Then the card count is displayed as 'Showing 1 - 5 of 7 items'
    When I click the 'Load more' button
    Then there are more discussion cards displayed
    Then the card count is displayed as 'Showing 1 - 7 of 7 items'


Scenario: FNHS50 - Navigate to next page of a discussion
    When I click the 'General Discussion Validation' link
    Then the 'General Discussion Validation' header is displayed 
    And the 'First Comment' comment card is displayed
    | AA         |
    | auto Admin |
    | 0 likes    |
    | Reply      |
    Then the card count is displayed as 'Showing 1 - 10 of 11 items'
    When I click the 'Next' link
    And the 'Eleventh Comment' comment card is displayed
    | AA         |
    | auto Admin |
    | 0 likes    |
    | Reply      |
    Then the card count is displayed as 'Showing 11 - 11 of 11 items'


@Core
Scenario: FNHS51 - Show more Replies
    When I click the 'General Discussion Validation' link
    Then the 'General Discussion Validation' header is displayed 
    And the 'First Comment' comment card is displayed
    | AA                |
    | auto Admin        |
    | 0 likes           |
    | Reply             |
    | Show more replies |
    And the 'First blank reply' reply card is displayed
    | AU                                        |
    | auto User                                 |
    | In response to auto Admin "First Comment" |
    | 0 likes                                   |
    | Reply                                     |
    When I open the 'Show more replies' accordion
    And the 'Second blank reply' reply card is displayed
    | AU                                        |
    | auto User                                 |
    | In response to auto Admin "First Comment" |
    | 0 likes                                   |
    | Reply                                     |
    And the 'Third blank reply' reply card is displayed
    | AU                                        |
    | auto User                                 |
    | In response to auto Admin "First Comment" |
    | 0 likes                                   |
    | Reply                                     |
    And the 'Fourth blank reply' reply card is displayed
    | AU                                        |
    | auto User                                 |  
    | In response to auto Admin "First Comment" |
    | 0 likes                                   |
    | Reply                                     |

@Core
Scenario: FNHS52 - Forum Navigate Back to Discussion Page
    When I click the 'General Discussion Validation' link
    Then the 'General Discussion Validation' header is displayed
    When I click the 'Back to discussions' link
    Then the 'All Discussions' header is displayed


Scenario: FNHS62 - Pinned Discussion Validation
    Then the 'General Discussion Validation' discussion card is displayed
    | AA                                     |
    | Created by Auto Admin [PRETTYDATE]     |
    | Last comment by Auto User [PRETTYDATE] |
    And the 'General Discussion Validation' discussion card is pinned