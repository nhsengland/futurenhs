Feature: forumAdmin
    User journeys covering functionality and navigation around forums as a Site/Forum Administrator

    Background:
        Given I have navigated to '/'
        And I have logged in as a 'group admin' and accept the cookies
        Then the 'My Groups' header is displayed
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I click the 'Forum' tab
        Then the 'All Discussions' header is displayed

    @Core
    Scenario Outline: FNHS58 - Create a new discussion
        When I click the 'New Discussion' link
        Then the 'Create Discussion' header is displayed
        When I enter '<title>' into the 'Title' field
        And I enter '<comment>' into the 'Comment' text editor
        And I click the 'Create Discussion' button
        Then the 'All Discussions' header is displayed
        Examples:
            | title              | comment                                  |
            | autoTestDiscussion | A brief description about the discussion |
            | [STRING: 100]      | 100 character limit test                 |


    Scenario Outline: FNHS59 - Create a discussion error validation, and cancel
        When I click the 'New Discussion' link
        Then the 'Create Discussion' header is displayed
        When I enter '<title>' into the 'Title' field
        And I enter '<comment>' into the 'Comment' text editor
        And I click the 'Create Discussion' button
        Then the '<error>' error message is displayed
        And the '<error>' error summary is displayed
        When I click the 'Discard Discussion' link
        Then the 'Entered Data will be lost' header is displayed
        When I click the 'Yes, discard' button
        Then the 'All Discussions' header is displayed
        Examples:
            | title         | comment | error                         |
            |               | comment | Enter the discussion title    |
            | title         |         | Enter the discussion comment  |
            | [STRING: 101] | comment | Enter 100 or fewer characters |


    Scenario: FNHS60 - Created discussion card validation
        Then the 'autoTestDiscussion' discussion card is displayed
            | AG                                           |
            | Created by Auto GroupAdmin [PRETTYDATE]      |
            | Last comment by Auto GroupAdmin [PRETTYDATE] |
            | Comments: 0                                  |
    # | Views: 0                                     |

    @Core
    Scenario: FNHS61 - Validate new discussion details
        When I click the 'autoTestDiscussion' link
        Then the 'autoTestDiscussion' header is displayed
        And the 'A brief description about the discussion' textual value is displayed
        Then the 'Join in the conversation' header is displayed
        And the 'You're signed in Auto GroupAdmin' textual value is displayed
        And the 'Your comment' label is displayed
        And the 'Add Comment' button is displayed