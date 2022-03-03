Feature: groupMembers
    User journeys covering managing/viewing members within a group

Background:
    Given I have navigated to '/'
    And I have logged in as a 'admin' and accept the cookies
    When I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed

@Core
Scenario: FNHS26 - Accept group member request
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    Then the 'Pending members' table exists
    When I click 'Accept' on the 'Auto User2' row of the 'Pending members' table
    Then the 'Auto User2' link is displayed
    And the 'Auto User2' row is displayed on the 'Group Members' table

@Core
Scenario: FNHS27 - Reject group member request
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    Then the 'Pending members' table exists
    When I click 'Reject' on the 'Auto User3' row of the 'Pending members' table
    Then the 'Pending members' table is not displayed

@Core
Scenario: FNHS28 - Add Registered User
    When I click the 'Automation Public Group' link
    Then the 'Automation Public Group' header is displayed
    When I select 'Add new member' from the group pages accordion
    Then the 'Invite member' header is displayed
    When I enter 'autoUser2@test.co.uk' into the 'New member email address' field
    And I click the 'Add new member' option
    Then the 'The email address belongs to a member of this group.' link is not displayed


Scenario Outline: FNHS29 - Add Registered User Error Validation
    When I click the 'Automation Public Group' link
    Then the 'Automation Public Group' header is displayed
    When I select 'Add new member' from the group pages accordion
    Then the 'Invite member' header is displayed
    When I enter '<input>' into the 'New member email address' field
    And I click the 'Add new member' option
    Then the '<error message>' error summary is displayed
    Then the '<error message>' error message is displayed
Examples:
    | input                | error message                                                                                                                                        |
    | fake@email           | Please provide a valid email address                                                                                                                 |
    | nouser@test.com      | This user is not registered on the platform. The platform is not open for new registrations at present, please contact support for more information. |
    | autoUser2@test.co.uk | The email address belongs to a member of this group.                                                                                                 |


Scenario: FNHS30 - View group member profile page
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    Then the 'Group members' table exists
    When I click 'auto User' on the 'auto User' row of the 'Group Members' table
    Then the 'Member Profile' header is displayed
    And the 'AU' textual value is displayed
    And the profile values are displayed
    | First name | auto |
    | Last name | User |
    | Email | autoUser@test.co.uk |
    # And the 'Last online' label is displayed
    # And the 'Joined' label is displayed
    # And the 'Member' checkbox is selected


Scenario Outline: FNHS93 - Change members role
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    Then the 'Group members' table exists
    When I click 'auto User' on the 'auto User' row of the 'Group Members' table
    Then the 'Member Profile' header is displayed
    When I open the 'Edit member' accordion
    When I select the '<new role>' radio button for 'Member role'
    When I click the 'Save Changes' button
    Then I confirm this on the open '' dialog
Examples:
    | initial role | new role |
    | Member       | Admin    |
    | Admin        | Member   |

@Pending
Scenario: FNHS94 - Remove member from a group
    When I click the 'Automation Admin Group' link
    Then the 'Automation Admin Group' header is displayed
    When I click the 'Members' tab
    Then the 'Group members' table exists
    When I click 'Auto User2' on the 'Auto User2' row of the 'Group Members' table
    Then the 'Member Profile' header is displayed
    When I click the 'Remove from group' button
    Then I confirm this on the open '' dialog
    And the 'Group administrators' table exists