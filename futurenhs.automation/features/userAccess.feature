Feature: User Access
    User journeys around users accessing the portal, including login page, registration, and unauthenticated page restrictions.

Background:
    Given I have navigated to '/'

@Core
Scenario Outline: FNHS00 - User login and log out
    Then the 'Log In' header is displayed
    And the 'Forgot your password?' link is displayed
    And the 'Register your interest' header is displayed
    And the 'Don't have an account?' textual value is displayed
    And the 'Register your interest' link is displayed
    When I enter '<email>' into the 'Email address' field
    And I enter '<password>' into the 'Password' field
    And I click the 'Log In' button
    Then the 'My Groups' header is displayed
    When I open the 'User Menu' accordion
    And I click the 'Log Off' link
    Then I confirm this on the open 'Logout' dialog
    Then the 'Log In' header is displayed
Examples:
   | email                | password   |
   | autoAdmin@test.co.uk | Tempest070 |
   | autoUser@test.co.uk  | Tempest070 |


Scenario Outline: FNHS01 - Access pages without authentication
    Given I have navigated to '<URL>'
    Then the '<Header>' header is displayed
Examples:
    | URL                   | Header               |
    | /terms-and-conditions | Terms and Conditions |
    | /privacy-policy       | Privacy Policy       |
    | /cookies              | Cookies              |
    | /contact-us           | Contact Us           |

@Core
Scenario: FNHS02 - Unauthenticated page redirect
    Given I have navigated to '/groups/aa/'
    Then the 'Log In' header is displayed

@Core @Pending
Scenario: FNHS03 - Invite User Form
    And I have logged in as a 'group admin' and accept the cookies
    Then the 'My Groups' header is displayed
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    When I select 'Invite new user' from the actions accordion
    Then the 'Invite new member to join this group' header is displayed
    When I enter 'autoTest@email.com' into the 'Email address' field
    And I enter 'autoTest@email.com' into the 'Repeat email address' field
    And I click the 'Send invite' option
    Then the 'The membership invitation has been sent' textual value is displayed

@Pending
Scenario Outline: FNHS04 - Invite User Error Validation
    Given I have logged in as a 'group admin' and accept the cookies
    Then the 'My Groups' header is displayed
    When I select the 'Automation Admin Group' group card
    Then the 'Automation Admin Group' header is displayed
    When I select 'Invite new user' from the actions accordion
    Then the 'Invite new member to join this group' header is displayed
    When I enter '<email>' into the 'Email address' field
    And I enter '<repeat email>' into the 'Repeat email address' field
    And I click the 'Send invite' option
    Then the '<error message>' error summary is displayed
    Then the '<error message>' error message is displayed
Examples:
    | email                   | repeat email            | error message                                                                                         |
    | auto2Test@email.com     | auto2Tet@email.com      | Please provide matching email addresses                                                               |
    | auto2Test@email.com     |                         | The Repeat email address field is required.                                                           |
    |                         | auto2Tet@email.com      | The Email address field is required.                                                                  |
    | fake@Email              | fake@Email              | Please provide a valid email address                                                                  |
    | autoEditUser@test.co.uk | autoEditUser@test.co.uk | A user with that email address is already registered on the platform - you may add them to your group |
    | autoAdmin@test.co.uk    | autoAdmin@test.co.uk    | A user with that email address is already a member of this group                                      |

@Core @Pending
Scenario: FNHS05 - Register as an invited user
    Given I have navigated to '/members/register' and accept the cookies
    Then the 'Register for an account' header is displayed
    And the 'Please read before choosing which address to use' textual value is displayed
    And the 'Use your work rather than personal email, where possible.' textual value is displayed
    And the 'Use the address provided to you by the main organisation you work for, where possible.' textual value is displayed
    And the 'Use your own email, not a group email address.' textual value is displayed
    When I enter 'autoTest@email.com' into the 'E-mail address' field
    And I enter 'Password101' into the 'Password' field
    And I enter 'Password101' into the 'Repeat password' field
    And I enter 'auto' into the 'First name' field
    And I enter 'test' into the 'Last name' field
    And I click the 'Register now' button
    Then the 'My Groups' header is displayed
    And the 'Automation Admin Group' link is displayed

@Pending
Scenario Outline: FNHS06 - User Registration Error Validation
    Given I have navigated to '/members/register' and accept the cookies
    Then the 'Register for an account' header is displayed
    When I enter '<email>' into the 'E-mail address' field
    And I enter '<password>' into the 'Password' field
    And I enter '<repeatpassword>' into the 'Repeat password' field
    And I enter '<firstname>' into the 'First name' field
    And I enter '<surname>' into the 'Last name' field
    And I click the 'Register now' button
    Then the 'There is a problem' header is displayed
    Then the '<error message>' error summary is displayed
    Then the '<error message>' error message is displayed
Examples:
    | email              | password    | repeatpassword | firstname | surname | error message                                                                           |
    | fake@Email         | Password101 | Password101    | auto      | test    | Please provide a valid email address                                                    |
    | autoTest@email.com | Password101 | Password101    | auto      | test    | This email address is already registered. Please provide another email address or login |
    | auto@test.co.uk    | password    | Password101    | auto      | test    | Your password must be at least 10 characters long                                       |
    | auto@test.co.uk    | Password101 | password111    | auto      | test    | Your passwords do not match                                                             |
    | auto@test.co.uk    | Password101 | Password101    |           | test    | Please provide your first name                                                          |

@Core @Pending
Scenario: FNHS07 - Attempt to register as an uninvited user
    Given I have navigated to '/members/register' and accept the cookies
    Then the 'Register for an account' header is displayed
    And the 'Please read before choosing which address to use' textual value is displayed
    And the 'Use your work rather than personal email, where possible.' textual value is displayed
    And the 'Use the address provided to you by the main organisation you work for, where possible.' textual value is displayed
    And the 'Use your own email, not a group email address.' textual value is displayed
    When I enter 'auto@Test.co.uk' into the 'E-mail address' field
    And I enter 'Password10' into the 'Password' field
    And I enter 'Password10' into the 'Repeat password' field
    And I enter 'auto' into the 'First name' field
    And I enter 'test' into the 'Last name' field
    And I click the 'Register now' button
    Then the 'This user has not been invited onto the platform. Please check the email address provided.' textual value is displayed


Scenario Outline: FNHS08 - Log In Error Validation
    Then the 'Log In' header is displayed
    When I enter '<email>' into the 'Email address' field
    And I enter '<password>' into the 'Password' field
    And I click the 'Log In' button
    Then the '<error message>' error message is displayed
Examples:
    | email                | password    | error message                  |
    |                      | Tempest2020 | The Username field is required |
    | autoAdmin@test.co.uk |             | The Password field is required |


Scenario: FNHS11 - Navigate to support site
    And I have logged in as a 'user' and accept the cookies
    Then the 'Need help?' textual value is displayed
    When I click the 'Visit our support site' link
    Then the 'https://support-futurenhs.zendesk.com/hc/en-gb' new tab is open and 'FutureNHS Support' is displayed
    
    
Scenario Outline: FNHS111 - Forgot password form validation
    Then the 'Log In' header is displayed
    When I click the 'Forgot your password?' link
    Then the 'Forgot Password' header is displayed
    When I enter '<input>' into the 'E-mail address' field
    And I click the 'Reset Password' button
    Then the '<header>' header is displayed
    And the '<content>' textual value is displayed
Examples:
    | input               | header                      | content                                                            |
    | autoUser@test.co.uk | Password reset request sent | An email has been sent with details on how to reset your password. |
    |                     | There is a problem          | Please provide a valid email address                               |