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
    

Scenario: FNHS113 - Manage users page validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage users' link
    Then the 'Users' header is displayed
    And the 'Admin Users' table is displayed
    | Name          | Role  | Date joined  | Last logged in | Actions   |
    | Admin Account | Admin | [PrettyDate] |                | Edit      |
    | auto Admin    | Admin | [PrettyDate] | [PrettyDate]   | Edit      |
    And the 'Invite user' link is displayed


Scenario: FNHS114 - Manage groups page validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage groups' link
    Then the 'Groups' header is displayed
    And the 'Admin Groups' table is displayed
    | Name                            | Members | Owner | Actions    |
    | 2021/22 Mental Health Workforce | 0       |       | Edit       |
    | Automation Admin Group          | 2       |       | Edit       |
    And the 'New group' link is displayed


Scenario: FNHS115 - Return to main admin page
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage groups' link
    Then the 'Groups' header is displayed
    When I click the 'Admin' breadcrumb
    Then the 'Admin' header is displayed
    And the 'Manage users' link is displayed
    And the 'Manage groups' link is displayed


@Core
Scenario: FNHS03 - Invite User Form
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage users' link
    Then the 'Users' header is displayed
    When I click the 'Invite user' link
    Then the 'Invite a new user' header is displayed
    And the 'Discard invite' link is displayed
    And the 'Send invite' button is displayed
    When I enter 'autoTest@email.com' into the 'Email address' field
    And I click the 'Send invite' button
    Then the 'The membership invitation has been sent' textual value is displayed



Scenario Outline: FNHS04 - Invite User Error Validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage users' link
    Then the 'Users' header is displayed
    When I click the 'Invite user' link
    Then the 'Invite a new user' header is displayed
    When I enter '<email>' into the 'Email address' field
    And I click the 'Send invite' button
    Then the '<error message>' error summary is displayed
    Then the '<error message>' error message is displayed
Examples:
    | email                   | error message               |
    |                         | Enter an email address      |
    | fake@Email              | Enter a valid email address |


@NotInLocal
Scenario Outline: FNHS116 - Invite User, email error validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage users' link
    Then the 'Users' header is displayed
    When I click the 'Invite user' link
    Then the 'Invite a new user' header is displayed
    When I enter '<email>' into the 'Email address' field
    And I click the 'Send invite' button
    Then the '<error message>' error summary is displayed
    Then the '<error message>' error message is displayed
Examples:
    | email                | error message        |
    | autoTest@email.com   | INVITE ALREADY SENT  |
    | autoAdmin@test.co.uk | EMAIL ALREADY EXISTS |


Scenario: FNHS117 - Discard Invite User
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage users' link
    Then the 'Users' header is displayed
    When I click the 'Invite user' link
    Then the 'Invite a new user' header is displayed
    And the 'Discard invite' link is displayed
    And the 'Send invite' button is displayed
    When I click the 'Discard invite' link
    Then the 'Entered Data will be lost' textual value is displayed
    And I confirm this on the open 'Discard Invite' dialog
    Then the 'Users' header is displayed


Scenario: FNHS118 - View Member Profile
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage users' link
    Then the 'Users' header is displayed
    And the 'Admin Users' table exists
    When I click 'auto User' on the 'auto User' row of the 'Admin Users' table
    Then the 'User Profile' header is displayed
    And the 'AU' textual value is displayed
    And the profile values are displayed
    | First name | auto |
    | Last name | User |
    | Email | autoUser@test.co.uk |
    # When I click the 'Back' breadcrumb
    # Then the 'Admin' header is displayed

# MISSING REQUIREMENTS FOR EDIT USER FEATURE
@Pending
Scenario Outline: FNHS119 - Edit user details page validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage users' link
    Then the 'Users' header is displayed
    And the 'Admin Users' table exists
    When I click 'Edit' on the 'auto User' row of the 'Admin Users' table
    Then the 'Edit Profile' header is displayed
    And the 'First name' field contains 'auto'
    And the 'Last name' field contains 'User'
    And the 'Email' field contains 'autouser@test.co.uk'
Examples:
    | user |
    | admin |
    | user |

@Pending
Scenario Outline: FNHS120 - Edit user details


Scenario: FNHS23 - Create a group 
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage groups' link
    Then the 'Groups' header is displayed
    And the 'Admin Groups' table exists
    When I click the 'New group' link
    Then the 'Create a group' header is displayed
    When I enter 'Group Creation Test' into the 'Group name' field
    And I enter 'A group created to test group creation' into the 'Strap line' text area
    When I upload the '/media/test.png' file
    Then the image file '/media/test.png' is uploaded and ready
    And I select the 'Theme' radio button for 'Choose your theme colour'
    # And I select 'autoAdmin' from the 'Group owner' dropdown
    # And I select 'autoAdmin' from the 'Group administrators' dropdown
    And I click the 'Save and create group' button
    Then the 'Admin' header is displayed


Scenario Outline: FNHS24 - Create a group error validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage groups' link
    Then the 'Groups' header is displayed
    And the 'Admin Groups' table exists
    When I click the 'New group' link
    Then the 'Create a group' header is displayed
    When I enter '<groupname>' into the 'Group name' field
    And I enter '<strapline>' into the 'Strap line' text area
    And I select the '<theme>' radio button for 'Choose your theme colour'
    And I enter '<owner>' into the 'Group owner' field
    And I enter '<admin>' into the 'Group administrators' field
    And I click the 'Save and create group' button
    Then the '<error message>' error summary is displayed
    And the '<error message>' textual value is displayed
Examples:
    | groupname | theme | strapline | owner | admin | error message          |
    |           | Theme |           |       |       | Enter the group name   |
    | mvG0bquv6bjDYjgmNqUy36FXCCO4deCOiFW9UQrQh51a8TjL0p1LYWvl3bPSkFBRcWgux3kDOosU2un0Mts0qmZxFfnJLznz0mSqoRbhsB7bDYUeG59DWuir78QvJkO0DFSHjWV2lnNN5GnQznTdyC48CRANTH0tZCJrxcVrQtONTyFePdtCQ122y0FPzshl7b9EMbUbFxIApWD8IJOnuGPLd6qb41PsB5Wy4T6dPLtJ6x3siiB9z3E0ns1gfiC0 | Theme | | | | Enter 255 or fewer characters |
    | title     | Theme | | mvG0bquv6bjDYjgmNqUy36FXCCO4deCOiFW9UQrQh51a8TjL0p1LYWvl3bPSkFBRcWgux3kDOosU2un0Mts0qmZxFfnJLznz0mSqoRbhsB7bDYUeG59DWuir78QvJkO0DFSHjWV2lnNN5GnQznTdyC48CRANTH0tZCJrxcVrQtONTyFePdtCQ122y0FPzshl7b9EMbUbFxIApWD8IJOnuGPLd6qb41PsB5Wy4T6dPLtJ6x3siiB9z3E0ns1gfiC0 | | Enter 255 or fewer characters |
    | title     | Theme | | | mvG0bquv6bjDYjgmNqUy36FXCCO4deCOiFW9UQrQh51a8TjL0p1LYWvl3bPSkFBRcWgux3kDOosU2un0Mts0qmZxFfnJLznz0mSqoRbhsB7bDYUeG59DWuir78QvJkO0DFSHjWV2lnNN5GnQznTdyC48CRANTH0tZCJrxcVrQtONTyFePdtCQ122y0FPzshl7b9EMbUbFxIApWD8IJOnuGPLd6qb41PsB5Wy4T6dPLtJ6x3siiB9z3E0ns1gfiC0 | Enter 255 or fewer characters |
    | title     | Theme | dG8M65IzuIXXAl7A9M6zMNpoD5YOTiEp6z5ePclY1GWlZcr3sS7uRgbTw513odptMGKkj1i7nHdo6LNdPSq4Ki2guLZj0viTsDt7zneYoNX7Gm703NkAIkCF1ezIuFoiEP8UyHYWW8alLPpzggPQxeRwuRVOJhIvEToZL6crTv1IUrnMU9JZ4xPr9BEO8DHnOv8VnfHqiW9CYZ4pxrftqkvTodTPi6k1QXGKIDekuphxCp2XjSNL34NoT1sWylnhFNTDkNjtfUzqIMu5XH8kbOumd9RCnljIJeLDVVinFQlkSFqH1IMl6dYGFEcAtxnPHG4F2tvDCrkKJbW7QMKRqso72L0m8fHhpB32EhFXr8bAhO9D1DOnSiTLrc1Xqtktu7EE037drEqDUzDJJTCAIFDv3FCX2l2qmnFc1oZ7sELqxWF8xwnxxbf8gcwCCEPcSc5NaKK7LEMX0jTPG0RE5N8m0hqf0tpuKx9C61sutwP4XaJUymAchBGE4EXcktlfAKT7t6ap5cFFXHEjKE03h7JOzrkfvUKlNn7KC6zCAP2i73WWtJO1biiQyCLWlHz3dCRuJxCRl3GZ8ruqS8WDibINehqD9sqrSnV2d4hQjSbpIoiZdAUaHU1rF8UfklouivYQoAMqHdEEM58XB159e7IxjbKq2NampE89exwokFhHfrFZ4FWFe3IHIy1mSLClRhWXizZutixgBd8S0YHikKd0r1r4eX12WM2B2SIi70jdpDy2aCXaeAlUecYX2V5IlHZUDvCrJtZhuvTEjJUy4rV3ZgkMc7HSbbvqi29NxGWWklsXwp7MwtbksKmaIiSIolELdJS7pcoqrwJFhhz6LRRw03te5IpE6VMO2NberTxLU9ir1avEGfl748SYAQHK0IWsHRfoRNgADjuceTsyw3gAWgQg5m3DVIScLGx6dUyOYhRWwemHtfNynpqN8xn9qyBko09GHjRm4sCEROAQaMHJGOOYU9lmwmtjRDCJR | | | Enter 1000 or fewer characters |


Scenario: FNHS120 - Create a group w/No Theme validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage groups' link
    Then the 'Groups' header is displayed
    And the 'Admin Groups' table exists
    When I click the 'New group' link
    Then the 'Create a group' header is displayed
    When I enter 'No Theme Test' into the 'Group name' field
    And I click the 'Save and create group' button
    Then the 'Select the group theme' error summary is displayed
    And the 'Select the group theme' textual value is displayed


Scenario: FNHS25 - Created Group Homepage Validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage groups' link
    Then the 'Groups' header is displayed
    And the 'Admin Groups' table exists
    And the 'Group Creation Test' row is displayed on the 'Admin Groups' table
    When I click 'Group Creation Test' on the 'Group Creation Test' row of the 'Admin Groups' table
    Then the 'Group Creation Test' header is displayed
    And the 'A group created to test group creation' textual value is displayed


Scenario Outline: FNHS121 - Edit group page validation
    And I have logged in as a 'admin' and accept the cookies
    When I open the 'User Menu' accordion
    And I click the 'Admin' link
    Then the 'Admin' header is displayed
    When I click the 'Manage groups' link
    Then the 'Groups' header is displayed
    And the 'Admin Groups' table exists
    And the 'Automation Editable Group' row is displayed on the 'Admin Groups' table
    When I click 'Edit' on the 'Automation Editable Group' row of the 'Admin Groups' table
    Then the 'Group name' field contains 'Automation Editable Group'
    And the 'Strap line' text area contains 'DO NOT USE - This group is reserved solely for use by our automated test scripts'
    And the 'Logo' label is displayed
    And the 'Choose your theme colour' fieldset is displayed
    And the 'Discard changes' link is displayed
    And the 'Save and close' button is displayed
    When I click the 'Discard changes' link
    Then the 'Entered Data will be lost' textual value is displayed
    And I confirm this on the open 'Discard Invite' dialog
    Then the 'Groups' header is displayed