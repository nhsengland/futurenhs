Feature: User Access
    User journeys around users viewing and managing cookies on the site.

Background:
    Given I have navigated to '/'
    And I have logged in as a 'user'


Scenario Outline: FNHS95 - Validate and select cookie banner option
    Then the 'Cookies on the FutureNHS website' header is displayed
    And the 'I'm OK with analytics cookies' button is displayed
    And the 'Do not use analytics cookies' button is displayed
    When I click the '<button>' button
    Then the 'Cookies on the FutureNHS website' header is not displayed
Examples:
    | button                        |
    | I'm OK with analytics cookies |
    | Do not use analytics cookies  |


Scenario: FNHS96 - Cookie consent banner, navigate to cookie policy
    Then the 'Cookies on the FutureNHS website' header is displayed
    And the 'read more about our cookies' link is displayed
    When I click the 'read more about our cookies' link
    Then the 'Cookies' header is displayed