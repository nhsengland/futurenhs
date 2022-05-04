Feature: groupAdmin
    User journeys covering Administrator functionality on managing a group

Background:
    Given I have navigated to '/'
    And I have logged in as a 'group admin' and accept the cookies
    Then the 'My Groups' header is displayed
     
@Pending
# PRIVATE GROUPS DON'T EXIST IN PRIVATEBETA/VNEXT
Scenario Outline: FNHS32 - Make a Group Public/Private
    When I select the 'Automation Editable Group' group card
    Then the 'Automation Editable Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    When I select the 'Is the group public?' checkbox
    And I click the 'Save and close' button
    Given I log off and log in as a 'user'
    Then the 'Automation Editable Group' header is displayed
    And the 'Join Group' link is displayed
    And I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    And the '<accesstext>' textual value is displayed
Examples:
    | accesstext                                                |
    | Currently no discussions in this Group                    |
    | You must be a member of this group to access this content |


Scenario: FNHS112 - Group Home page manager available
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    And the 'DO NOT USE - This group is reserved solely for use by our automated test scripts' textual value is displayed
    # And the '' textual value is displayed
    When I open the 'Actions' accordion
    Then the 'Page manager' link is displayed


Scenario: FNHS113 - Group Members table admin validation    
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    And the 'Group Members' table is displayed
    | Name            | Role             | Date joined  | Last logged in | Actions |
    | Auto GroupAdmin | Admin            | [PrettyDate] | [PrettyDate]   | Edit    |
    # | auto Test  | Standard Members | [PrettyDate] | [PrettyDate]   | Edit |  
    | auto User       | Standard Members | [PrettyDate] | [PrettyDate]   | Edit    |