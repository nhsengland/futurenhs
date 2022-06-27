Feature: groupPrivate
    User journeys covering functionality and permissions when working with a private group

    Background:
        Given I have navigated to '/'


    Scenario: FNHS37 - Pending Members Table Validation
        Given I have logged in as a 'group admin' and accept the cookies
        Then the 'My Groups' header is displayed
        When I select the 'Automation Private Group' group card
        Then the 'Automation Private Group' header is displayed
        When I click the 'Members' tab
        And the 'Pending members' table is displayed
            | Name       | Email                | Request date | Actions        |
            | Auto User3 | autoUser3@test.co.uk | [PrettyDate] | Accept\nReject |


    Scenario: FNHS38 - Create a private group
        Given I have logged in as an 'admin' and accept the cookies
        And I have navigated to '/admin/groups/create'
        Then the 'Create a group' header is displayed
        When I enter 'AutoNew Private Group' into the 'Group name' field
        When I enter 'DO NOT USE - This group is reserved solely for use by our automated test scripts' into the 'Strap line' field
        And I select the 'Theme' radio button for 'Choose your theme colour'
        And I choose 'Auto GroupAdmin' from the 'Group owner' auto suggest list
        And I select the 'Group is public?' checkbox
        And I click the 'Save and create group' button
        Then the 'Groups' header is not displayed
        And the 'Admin groups' table exists
        And I click the 'AutoNew Private Group' link
        Then the 'AutoNew Private Group' header is displayed
        When I click the 'Members' tab
        Then the 'Pending Members' header is displayed
        And the 'This group currently has no outstanding membership requests' textual value is displayed
        

    Scenario: FNHS39 - Private Group Change Warning Validation
        Given I have logged in as a 'group admin' and accept the cookies
        Then the 'My Groups' header is displayed
        When I select the 'Automation Public Group' group card
        Then the 'Automation Public Group' header is displayed
        When I select 'Edit group information' from the group actions accordion
        Then the 'Edit group information' header is displayed
        And the 'Group is public?' textual value is displayed
        Then I select the 'Group is public?' checkbox
        And I click the 'Save and close' button 
        Then the 'Change group privacy' header is displayed
        And the 'will set this group to private, restricting access to approved members only. This cannot be undone, are you sure you wish to continue?' textual value is displayed
        And I cancel this on the open dialog


    Scenario: FNHS40 - Non Member Group View Validation
        Given I have logged in as a 'user' and accept the cookies
        Then the 'My Groups' header is displayed
        And I have navigated to '/groups/automation-private-group'
        Then the 'Automation Private Group' header is displayed
        And the 'About us' tab is displayed
        And the 'About Us' header is displayed
        And the 'Home' tab is not displayed
        And the 'Forum' tab is not displayed
        And the 'Files' tab is not displayed
        And the 'Members' tab is not displayed


    Scenario: FNHS41 - Join a private group
        Given I have logged in as a 'user' and accept the cookies
        Then the 'My Groups' header is displayed
        And I have navigated to '/groups/automation-private-group'
        Then the 'Automation Private Group' header is displayed
        And the 'Join Group' link is displayed
        When I click the 'Join Group' link
        Then the 'Awaiting Approval' textual value is displayed