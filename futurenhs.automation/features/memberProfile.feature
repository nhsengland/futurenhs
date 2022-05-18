Feature: Member Profile
    User journeys around users accessing and modifying their own personal information within FNHS

Background:
    Given I have navigated to '/'
	And I have logged in as an 'edituser' and accept the cookies
	Then the 'My Groups' header is displayed
    When I open the 'User Menu' accordion
    And I click the 'My profile' link
    Then the 'User Profile' header is displayed


Scenario: FNHS78 - View my profile page
    And the 'AU' textual value is displayed
    And the profile values are displayed
    | First name | autoEdit |
    | Last name  | User     |
    And the 'Edit profile' link is displayed


Scenario: FNHS13 - Edit my profile page validation
    And the 'Edit profile' link is displayed
    When I click the 'Edit profile' link
    Then the 'Edit profile' header is displayed
    And the 'Image' label is displayed
    And the 'The selected file must be a JPG or PNG and must be smaller than 5MB.' textual value is displayed
    And the 'First name' field contains 'autoEdit'
    And the 'Last name (optional)' field contains 'User'
    And the 'Preferred pronouns (optional)' label is displayed
    And the 'Please confirm that all changes are in line with the platforms terms and conditions' textual value is displayed
    And the 'Discard changes' link is displayed
    And the 'Save changes' button is displayed


Scenario: FNHS79 - Edit my profile avatar upload
    When I click the 'Edit profile' link
    Then the 'Edit profile' header is displayed
    And the 'Image' label is displayed
    And I upload the '/media/largeimage.png' file
    Then the image file '/media/largeimage.png' is uploaded and ready
    And I select the 'Please confirm that all changes are in line with the platforms terms and conditions' checkbox
    And I click the 'Save changes' button
    Then the 'User Profile' header is displayed
    And the profile values are displayed
    | First name | autoEdit |
    | Last name  | User     |


Scenario Outline: FNHS80 - Edit my profile
    When I click the 'Edit profile' link
    Then the 'Edit profile' header is displayed    
    When I enter '<input>' into the '<label>' field
    And I select the 'Please confirm that all changes are in line with the platforms terms and conditions' checkbox
    And I click the 'Save changes' button
    Then the 'User Profile' header is displayed
    And the '<input>' textual value is displayed
Examples:
    | input         | label              |
    | him           | Preferred pronouns |
    | [STRING: 255] | Last name          |
    | Name          | Last name          |
    | [STRING: 255] | Preferred pronouns |
    | [STRING: 255] | First name         |
    | New           | First name         |


Scenario Outline: FNHS81 - Edit my profile error validation
    When I click the 'Edit profile' link
    Then the 'Edit profile' header is displayed
    And I enter '<firstname>' into the 'First name' field
    And I enter '<lastname>' into the 'Last name' field
    And I enter '<pronoun>' into the 'Preferred pronouns' field
    And I select the 'Please confirm that all changes are in line with the platforms terms and conditions' checkbox
    And I click the 'Save changes' button
    Then the '<error message>' textual value is displayed
Examples:
    | firstname     | lastname      | pronoun       | error message                 |
    |               | lastname      | him           | Enter a name                  |
    | [STRING: 256] | lastname      | him           | Enter 255 or fewer characters |
    | firstname     | [STRING: 256] | him           | Enter 255 or fewer characters |
    | firstname     | lastname      | [STRING: 256] | Enter 255 or fewer characters |


Scenario: FNHS82 - Edit my profile w/o accepting T&Cs
	Then the 'My Groups' header is displayed
    When I open the 'User Menu' accordion
    And I click the 'My profile' link
    Then the 'User Profile' header is displayed
    When I click the 'Edit profile' link
    Then the 'Edit profile' header is displayed
    When I click the 'Save changes' button
    Then the 'Select to confirm the terms and conditions' error summary is displayed
    And the 'Select to confirm the terms and conditions' textual value is displayed
