Feature: mobileNavigation
    Tests covering unique mobile manu navigation functionality available to a user access the site via mobile web app

    Background: Log In
        Given I have navigated to '/'
        And I ensure the browser is in mobile emulation
        Then the 'Log In' header is displayed
        And the 'Register your interest' header is displayed
        And I have logged in as an 'group admin' and accept the cookies

    @Core
    Scenario: FNHS:M01 - Menu Validation
        When I open the 'Mobile Menu' accordion
        Then the 'Auto GroupAdmin' textual value is displayed
        And the 'Groups' link is displayed
        And the 'Need help?' textual value is displayed
        And the 'Visit our support site' link is displayed
        When I open the 'user menu' accordion
        Then the 'My profile' link is displayed
        And the 'Log Off' link is displayed

    @Core
    Scenario: FNHS:M02 - All Groups Validation
        Then the 'My Groups' header is displayed
        Then the 'Collaborate without boundaries' header is displayed
        When I open the 'group pages' accordion
        And I click the 'Discover new groups' link
        Then the 'Discover new Groups' header is displayed

    @Core
    Scenario: FNHS:M03 - Group Members Table Validation
        Then the 'My Groups' header is displayed
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I select 'Members' from the group menu accordion
        Then the 'Group Members' mobile table is displayed
            | Auto GroupAdmin |                  |
            | Role            | Admin            |
            | Date joined     | [PrettyDate]     |
            | Last logged in  | [PrettyDate]     |
            | Edit            |                  |
            | auto User       |                  |
            | Role            | Standard Members |
            | Date joined     | [PrettyDate]     |
            | Last logged in  | [PrettyDate]     |
            | Edit            |                  |