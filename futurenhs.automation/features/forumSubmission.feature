Feature: Forum Submission
    User Journeys around submissions within a forum as a standard user on the Future NHS platform

Background:
    Given I have navigated to '/'
    And I have logged in as a 'user'
    When I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed 
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed 
    When I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    When I click the 'forumSubmission Discussion' link
    Then the 'forumSubmission Discussion' header is displayed 

@Core
Scenario: FNHS53 - Post a comment in a discussion (2570)
    When I enter 'Comment posted by the automation' into the text editor
    And I click the 'Add comment' option
    Then the 'Comment posted by the automation' comment card is displayed
    | AU        |
    | Auto User |
    | Just now  |


Scenario: FNHS54 - Post a comment error validation
    And I click the 'Add comment' option
    Then the 'Please provide a comment.' error message is displayed

@Core
Scenario: FNHS55 - Reply to existing comment
    Then the 'This is a comment to reply to' comment card is displayed
    | AA            |
    | Auto Admin    |
    | 0 likes Reply |
    When I click reply on the 'This is a comment to reply to' comment card
    And I enter 'This is a reply' into the text editor
    And I click the 'Add comment' option
    Then the 'This is a reply' reply card is displayed
    | AU                                             |
    | Auto User                                      |
    | Reply to Auto Admin “This is a comment to ...” |


Scenario: FNHS56 - Reply to a reply
    Given I log off and log in as an 'admin'
    Then the 'forumSubmission Discussion' header is displayed
    Then the 'This is a reply' reply card is displayed
    | AU                                             |
    | Auto User                                      |
    When I click reply on the 'This is a reply' reply card
    And I enter 'This is another reply' into the text editor
    And I click the 'Add comment' option
    Then the 'This is another reply' reply card is displayed
    | AA                                    |
    | Auto Admin                            |
    | Reply to Auto User “This is a reply ” |


Scenario: FNHS57- Clear comment text before posting
    When I enter 'New comment post by user' into the text editor
    And I click the 'Clear' button
    Then the text editor is empty 