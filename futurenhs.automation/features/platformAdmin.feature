Feature: Group Create
    User journeys covering Administrator functionality on creating a new group

Background:
    Given I have navigated to '/'

    
Scenario: FNHS20 - Admin page validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    And the 'Manage users' link is displayed
    And the 'Manage groups' link is displayed


Scenario Outline: FNHS57 - Admin page permission 
    And I have logged in as a '<user>' and accept the cookies
    When I open the 'User Menu' accordion
    Then the 'Admin' link <visibility> displayed
Examples:
    | user        | visibility |
    | admin       | is         |
    | group admin | is not     | 
    | user        | is not     |

@Pending
#REFACTOR FOR PRIVATE BETA
Scenario: FNHS23 - Create a group 
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    When I click the 'Admin' link
    Then the 'MvcForum' link is displayed
    When I click the 'Groups' link
    Then the 'All Groups' header is displayed
    When I click the 'Create New' link
    When I enter 'Group Creation Test' into the 'Group Name' field
    And I enter 'A group created to test group creation' into the 'Strap line' text area
    And I enter 'Subtitle for created group' into the 'Group subtitle' field
    And I enter 'This tests the introduction filed works as expected' into the 'Group introduction' text area
    And I select 'autoAdmin' from the 'Group Owner' dropdown
    And I select 'autoAdmin' from the 'Group Administrators' dropdown
    And I click the 'Create' option
    Then the 'All Groups' header is displayed

@Pending
#REFACTOR FOR PRIVATE BETA
Scenario Outline: FNHS24 - Create a group error validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    When I click the 'Admin' link
    Then the 'MvcForum' link is displayed
    When I click the 'Groups' link
    Then the 'All Groups' header is displayed
    When I click the 'Create New' link
    When I enter '<groupname>' into the 'Group Name' field
    And I enter '<strapline>' into the 'Strap line' text area
    And I enter '<subtitle>' into the 'Group subtitle' field
    And I enter '<introduction>' into the 'Group introduction' text area
    And I select 'autoAdmin' from the 'Group Owner' dropdown
    And I select 'autoAdmin' from the 'Group Administrators' dropdown
    And I click the 'Create' option
    Then the '<error message>' error summary is displayed
    And the '<error message>' textual value is displayed
Examples:
    | groupname | strapline | subtitle                                                                                                                                                                                                                                                         | introduction                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | error message                                                    |
    | title     | strapline | XepnVExSLqKzV7AK4UbuqeSRYQ8U0vvC0Qa1XhbSXuGV4ps6xjQWdjg2BDWP2icZUZQ10QPib9PGPAmG4oaRdZczkcnwz9NsPWagLBYSKWEWMBIfg4mVcN44ARPCJLJ5KSjGl6lVe641Rq7Gmqgqv7r7FfqXZd96roBGcFRBwMEbuYJnOB6TCLyQMDXYpDZGZOVikfxoUzUFBEDloc1ilgFAsiL0kh3lcKrCIkKFwJDOye7q5j1kzMsXZ7JUDcx | intro                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            | The group subtitle must be less than 255 characters              |
    | title     | strapline | subtitle                                                                                                                                                                                                                                                        |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | Please provide an introduction to your group                     |
    | title     | strapline | subtitle                                                                                                                                                                                                                                                        | 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 | The group introduction must not be greater than 4000 characters. |

@Pending
Scenario: FNHS25 - Created Group Homepage Validation
    And I have logged in as a 'admin' and accept the cookies
    Then the 'My Groups' header is displayed
    When I click the 'Group Creation Test' link
    Then the 'Group Creation Test' header is displayed
    And the 'A group created to test group creation' textual value is displayed
    And the 'Subtitle for created group' textual value is displayed
    And the 'This tests the introduction filed works as expected' textual value is displayed