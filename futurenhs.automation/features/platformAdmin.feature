Feature: Group Create
    User journeys covering Administrator functionality on creating a new group

    Background:
        Given I have navigated to '/'
        And I have logged in as a 'admin' and accept the cookies
        When I open the 'User Menu' accordion
        And I click the 'Admin' link
        Then the 'Admin' header is displayed

    @Core
    Scenario: FNHS20 - Admin pages validation
        And the 'Manage users' link is displayed
        And the 'Manage groups' link is displayed
        When I click the 'Manage users' link
        Then the 'Users' header is displayed
        And the 'Admin users' table exists
        When I click the 'Admin' breadcrumb
        Then the 'Admin' header is displayed
        When I click the 'Manage groups' link
        Then the 'Groups' header is displayed
        And the 'Admin groups' table exists

    @NotInLocal
    # MVCForum bug when re logging going to 8888 not 5000
    Scenario Outline: FNHS57 - Admin page permission
        Given I have logged off as the current user
        And I have logged in as a '<user>' and accept the cookies
        When I open the 'User Menu' accordion
        Then the 'Admin' link <visibility> displayed
        Examples:
            | user        | visibility |
            | admin       | is         |
            | group admin | is not     |
            | user        | is not     |


    Scenario: FNHS03 - Invite User Page Validation
        When I click the 'Manage users' link
        Then the 'Users' header is displayed
        And the 'Invite user' link is displayed
        When I click the 'Invite user' link
        Then the 'Invite a new user' header is displayed
        And the 'Email address' label is displayed
        And the 'Discard invite' link is displayed
        And the 'Send invite' button is displayed


    Scenario Outline: FNHS04 - Invite User Error Validation
        When I click the 'Manage users' link
        Then the 'Users' header is displayed
        And the 'Invite user' link is displayed
        When I click the 'Invite user' link
        Then the 'Invite a new user' header is displayed
        When I enter '<email>' into the 'Email address' field
        And I click the 'Send invite' button
        Then the '<error message>' error summary is displayed
        Then the '<error message>' error message is displayed
        Examples:
            | email      | error message               |
            |            | Enter an email address      |
            | fake@Email | Enter a valid email address |


    Scenario Outline: FNHS23 - Create a group
        When I click the 'Manage groups' link
        Then the 'Groups' header is displayed
        And the 'New group' link is displayed
        When I click the 'New group' link
        Then the 'Create a group' header is displayed
        And the 'Discard group' link is displayed
        When I enter '<groupname>' into the 'Group name' field
        And I enter '<strapline>' into the 'Strap line' field
        And I select the 'Theme' radio button for 'Choose your theme colour'
        And I choose 'auto Admin' from the 'Group owner' auto suggest list
        And I choose 'auto Admin' from the 'Group administrators' auto suggest list
        And I click the 'Save and create group' button
        Then the 'Groups' header is not displayed
        And the 'Admin groups' table exists
        And the '<groupname>' row is displayed on the 'Admin groups' table
        Examples:
            | groupname                | strapline                              |
            | Automation Created Group | A group created to test group creation |
            | [STRING: 255]            | name char length test                  |
            | Auto 255 Strapline Test  | [STRING: 255]                          |


    Scenario Outline: FNHS24 - Create a group error validation
        When I click the 'Manage groups' link
        Then the 'Groups' header is displayed
        And the 'New group' link is displayed
        When I click the 'New group' link
        Then the 'Create a group' header is displayed
        And the 'Discard group' link is displayed
        When I enter '<groupname>' into the 'Group name' field
        And I enter '<strapline>' into the 'Strap line' field
        And I choose '<owner>' from the 'Group owner' auto suggest list
        And I click the 'Save and create group' button
        Then the '<error message>' error summary is displayed
        And the '<error message>' textual value is displayed
        Examples:
            | groupname     | strapline     | owner      | error message                 |
            |               | strapline     | auto Admin | Enter the group name          |
            | [STRING: 256] | strapline     | auto Admin | Enter 255 or fewer characters |
            | groupname     | [STRING: 256] | auto Admin | Enter 255 or fewer characters |
            | groupname     | strapline     | auto Admin | Select the group theme        |
            | groupname     | strapline     |            | Enter a valid group owner     |


    Scenario: FNHS25 - Created Group Homepage Validation
        Given I return to the homepage
        Then the 'Automation Created Group' group card is displayed
            | A group created to test group creation |
            | Members: 1Discussions: 0               |
        When I click the 'Automation Created Group' link
        Then the 'Automation Created Group' header is displayed
        And the 'A group created to test group creation' textual value is displayed

    Scenario Outline: FNHS123 - Manage Member Edit Role
        When I click the 'Manage users' link
        Then the 'Users' header is displayed
        When I click 'Edit' on the 'auto User' row of the 'admin users' table
        Then the 'Edit profile' header is displayed
        And the 'Edit user role' header is displayed
        And the 'Update role' button is displayed
        When I select the '<role>' radio button for 'Choose user's platform role'
        And I click the 'Update role' button
        And the 'User Profile' header is displayed
        When I click the 'Users' breadcrumb
        Then the 'Users' header is displayed
        And the following row is displayed on the 'admin users' table
            | auto User | <role> | [PrettyDate] | [PrettyDate] | Edit |
        Examples:
            | role             |
            | Admin            |
            | Guest            |
            | Standard Members |
