Feature: Forum Submission
    User Journeys around submissions within a forum as a standard user on the Future NHS platform

    Background:
        Given I have navigated to '/'
        And I have logged in as a 'user' and accept the cookies
        Then the 'My Groups' header is displayed
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I click the 'Forum' tab
        Then the 'All Discussions' header is displayed

    @Core
    Scenario Outline: FNHS48 - Like/Unlike a comment
        When I select the 'General Discussion Validation' discussion card
        Then the 'General Discussion Validation' header is displayed
        And the 'Comment for Like test' comment card is displayed
            | AA                 |
            | auto Admin         |
            | <Pre Action State> |
            | Reply              |
        When I <Action Type> the 'Comment for Like test' comment card
        Then the 'Comment for Like test' comment card is displayed
            | AA                  |
            | auto Admin          |
            | <Post Action State> |
            | Reply               |
        Examples:
            | Pre Action State | Action Type | Post Action State |
            | 0 likes          | like        | 1 like            |
            | 1 like           | unlike      | 0 likes           |

    @Core
    Scenario: FNHS53 - Post a comment in a discussion
        When I select the 'forumSubmission Discussion' discussion card
        Then the 'forumSubmission Discussion' header is displayed
        When I enter 'Comment posted by the automation' into the 'Your comment' text editor
        And I click the 'Add Comment' button
        Then the 'Comment posted by the automation' comment card is displayed
            | AU           |
            | auto User    |
            | [PRETTYDATE] |
            | 0 likes      |
            | Reply        |


    Scenario: FNHS12 - Validate Discussion card comment counter
        Then the 'forumSubmission' discussion card is displayed
            | AA                                      |
            | Created by Auto Admin [PRETTYDATE]      |
            | Last comment by Auto Admin [PRETTYDATE] |
            | Comments: 3                             |
        When I select the 'forumSubmission Discussion' discussion card
        Then the 'forumSubmission Discussion' header is displayed
        When I enter 'Comment posted by the automation' into the 'Your comment' text editor
        And I click the 'Add Comment' button
        Then the 'Comment posted by the automation' comment card is displayed
            | AU           |
            | auto User    |
            | [PRETTYDATE] |
            | 0 likes      |
            | Reply        |
        When I click the 'Back to discussions' link
        Then the 'forumSubmission' discussion card is displayed
            | AA                                      |
            | Created by Auto Admin [PRETTYDATE]      |
            | Last comment by Auto Admin [PRETTYDATE] |
            | Comments: 4                             |


    Scenario Outline: FNHS54 - Post a comment error validation
        When I select the 'forumSubmission Discussion' discussion card
        Then the 'forumSubmission Discussion' header is displayed
        When I enter '<input>' into the 'Your comment' text editor
        And I click the 'Add Comment' button
        And I click the 'Add Comment' button
        Then the '<error>' error message is displayed
        Examples:
            | input | error              |
            |       | Enter your comment |
    # | | Enter 100,000 or fewer characters | UNABLE TO ADD 100K characters into app

    @Core
    Scenario: FNHS55 - Reply to existing comment
        When I select the 'forumSubmission Discussion' discussion card
        Then the 'forumSubmission Discussion' header is displayed
        Then the 'This is a comment to reply to' comment card is displayed
            | AA         |
            | auto Admin |
            | 0 likes    |
            | Reply      |
        When I click reply on the 'This is a comment to reply to' comment card
        Then the 'Your reply' label is displayed
        And I enter 'This is a reply' into the 'Your reply' text editor
        And I click the 'Reply' button
        Then the 'This is a reply' reply card is displayed
            | AU                                             |
            | auto User                                      |
            | In response to auto Admin "This is a comme..." |
            | 0 likes                                        |
            | Reply                                          |

    Scenario: FNHS56 - Reply to a reply
        When I select the 'forumSubmission Discussion' discussion card
        Then the 'forumSubmission Discussion' header is displayed
        Then the 'Reply for FNHS56 test scenario' reply card is displayed
            | AA         |
            | auto Admin |
            | 0 likes    |
            | Reply      |
        When I click reply on the 'Reply for FNHS56 test scenario' reply card
        And I enter 'This is another reply' into the 'Your reply' text editor
        And I click the 'Reply' button
        Then the 'This is another reply' reply card is displayed
            | AU                                             |
            | auto User                                      |
            | In response to auto Admin "Reply for FNHS5..." |
            | 0 likes                                        |
            | Reply                                          |