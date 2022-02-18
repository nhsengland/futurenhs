Feature: User Access
    User journeys around users viewing and managing cookies on the site.

Background:
    Given I have navigated to '/'
    And I have logged in as a 'user'

@Pending
Scenario: FNHS95 - Validate and Accept Cookie Consent Banner
    Then the 'Cookies on the FutureNHS website' header is displayed
    And the 'I'm okay with analytics cookies' button is displayed
    And the 'Do not use analytics cookies' button is displayed
    When I click the 'I'm okay with analytics cookies' button
    Then the 'Cookies on the FutureNHS website' header is not displayed

@Pending
Scenario: FNHS96 - Cookie consent banner, navigate to cookie policy
    Then the 'Cookies on the FutureNHS website' header is displayed
    And the '' link is displayed
    When I click the '' link
    Then the 'Cookies' header is displayed