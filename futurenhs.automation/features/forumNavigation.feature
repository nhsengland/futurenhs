Feature: Forum Navigation
    User Journeys around general forum navigation as a standard member on the Future NHS platform


Background:
    Given I have navigated to '/'
    And I have logged in as a 'user'
    When I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed 


@Core
Scenario Outline: FNHS48 - Like/Unlike a comment
    When I click the 'General Discussion Validation' link
    Then the 'General Discussion Validation' header is displayed 
    And the 'Comment for Like test' comment card is displayed
    | AA                    |
    | Auto Admin            |
    | <Pre Action State>    |
    When I <Action Type> the 'Comment for Like test' comment card 
    Then the 'Comment for Like test' comment card is displayed
    | AA                    |
    | Auto Admin            |
    | <Post Action State>   |
Examples:
    | Pre Action State | Action Type | Post Action State |
    | 0 likes Reply    | like        | 1 likes Reply     |
    | 1 likes Reply    | unlike      | 0 likes Reply     |

@Core
Scenario: FNHS49 - Load more Comments is available
    When I click the 'General Discussion Validation' link
    Then the 'General Discussion Validation' header is displayed 
    And the 'Load more' button is displayed
    And there are comment cards displayed
    When I click the 'Load more' button
    Then there are more comment cards displayed


Scenario: FNHS50 - Load more replies does not exist
    When I click the 'forumSubmission Discussion' link
    Then the 'forumSubmission Discussion' header is displayed 
    And the 'Unable to reply to comment test' comment card is displayed
    | AU                              |
    | Auto User                       |
    | 0 likes                         |
    And there are no replies available on the 'Unable to reply to comment test' comment card

@Core
Scenario: FNHS51 - Load more Replies
    When I click the 'General Discussion Validation' link
    Then the 'General Discussion Validation' header is displayed 
    And the 'First Comment' comment card is displayed
    | AA                |
    | Auto Admin        |
    | 0 likes Reply     |
    | Load more replies |
    And the 'First blank reply' reply card is displayed
    | AU                                   |
    | Auto User                            |
    | Reply to Auto Admin “First Comment ” |
    | 0 likes                              |
    When I click the 'Load more replies' button
    And the 'Second blank reply' reply card is displayed
    | AU                                   |
    | Auto User                            |
    | Reply to Auto Admin “First Comment ” |
    | 0 likes                              |
    And the 'Third blank reply' reply card is displayed
    | AU                                   |
    | Auto User                            |
    | Reply to Auto Admin “First Comment ” |
    | 0 likes                              |
    And the 'Fourth blank reply' reply card is displayed
    | AU                                   |
    | Auto User                            |
    | Reply to Auto Admin “First Comment ” |
    | 0 likes                              |

@Core
Scenario: FNHS52 - Forum Load More Discussions
    Then there are discussion cards displayed
    When I click the 'Load more' button
    Then there are more discussion cards displayed