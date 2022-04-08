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
Scenario: FNHS58 - Create a new discussion
    When I click the 'New Discussion' link
    Then the 'Create Discussion' header is displayed
    When I enter 'autoTestDiscussion' into the 'Title' field
    And I enter 'A brief description about the discussion' into the 'Comment' text editor
    Then the 'All Discussions' header is displayed
    

Scenario: FNHS59 - Create a discussion error validation, and cancel
    When I click the 'New Discussion' link
    Then the 'Create Discussion' header is displayed
    When I click the 'Create Discussion' button
    Then the 'Enter the discussion title' error message is displayed
    And the 'Enter the discussion comment' error message is displayed
    And the 'Enter the discussion title' error summary is displayed
    And the 'Enter the discussion comment' error summary is displayed
    When I click the 'Discard Discussion' link
    Then the 'Entered Data will be lost' header is displayed
    When I click the 'Yes, discard' button
    Then the 'All Discussions' header is displayed


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