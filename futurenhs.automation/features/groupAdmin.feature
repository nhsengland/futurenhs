Feature: groupAdmin
    User journeys covering Administrator functionality on managing a group

    Background:
        Given I have navigated to '/'
        And I have logged in as a 'group admin' and accept the cookies
        Then the 'My Groups' header is displayed

    
    Scenario: FNHS32 - Make a Group Private
        When I select the 'Automation Group To Be Restricted' group card
        Then the 'Automation Group To Be Restricted' header is displayed
        When I select 'Edit group information' from the actions accordion
        Then the 'Edit group information' header is displayed
        And the 'Group is public?' textual value is displayed
        When I select the 'Group is public?' checkbox
        And I click the 'Save and close' button
        And I confirm this on the open dialog
        Then the 'Automation Group To Be Restricted' header is displayed
        When I click the 'Members' tab
        Then the 'Pending Members' header is displayed
        And the 'This group currently has no outstanding membership requests' textual value is displayed
        And the 'Group Members' table exists

@NotInLocal
    Scenario Outline: FNHS114 - Edit Page Permissions Validation
        Given I have logged off as the current user
        And I have logged in as a '<user>'
        And I have navigated to '/groups/automation-editable-group'
        And the 'You are a Group Admin of this page. Please click edit to switch to editing mode' textual value <visibility> displayed
        And the 'Edit page' button <visibility> displayed
        Examples:
            | user        | visibility |
            | group admin | is         |
            | admin       | is         |
            | user        | is not     |


    Scenario: FNHS113 - Group Members table admin validation
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I click the 'Members' tab
        And the 'Group Members' table is displayed
            | Name            | Role             | Date joined  | Last logged in | Actions |
            | Auto GroupAdmin | Admin            | [PrettyDate] | [PrettyDate]   | Edit    |
            # | auto Test  | Standard Members | [PrettyDate] | [PrettyDate]   | Edit |
            | auto User       | Standard Members | [PrettyDate] | [PrettyDate]   | Edit    |