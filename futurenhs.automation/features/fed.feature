Feature: FED Accessibility and Performance
    Blurb about the feature and test coverage


Scenario Outline: FNHS:FED01 - Lighthouse Performance Test
    Given I have navigated to '/'
    And I have logged in as a 'user'
    When I have navigated to '<url>'
    Then the page is performant and follows best practices
Examples:
    | url |
    | / |
    | groups/ |
    | groups/aa/ |
    | groups/aa/members |
    | groups/aa/members/d74ed860-9ea5-4c95-9394-ad3a00924fa5 |
    | groups/aa/folders |
    | groups/aa/folders/create |
    | groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb |
    | groups/aa/files/54249d0f-3544-4b76-b6fb-ae0001022c09 |
    | groups/aa/forum |
    | groups/aa/forum/create |
    | groups/aa/forum/3fbc0d4e-c2a7-46a0-99e9-ad7f010eb4ad |
    # | groups/aa/update |
    | groups/discover |


Scenario Outline: FNHS:FED02 - Axe Accessibility Test
    Given I have navigated to '/'
    And I have logged in as a 'user'
    Given I have navigated to '<url>'
    Then I ensure the page is accessible
Examples:
    | url |
    | / |
    | groups/ |
    | groups/aa/ |
    | groups/aa/members |
    | groups/aa/members/d74ed860-9ea5-4c95-9394-ad3a00924fa5 |
    | groups/aa/folders |
    | groups/aa/folders/create |
    | groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb |
    | groups/aa/files/54249d0f-3544-4b76-b6fb-ae0001022c09 |
    | groups/aa/forum |
    | groups/aa/forum/create |
    | groups/aa/forum/3fbc0d4e-c2a7-46a0-99e9-ad7f010eb4ad |
    # | groups/aa/update |
    | groups/discover |

@Pending
Scenario Outline: Authenticated user - FuturneNHS full page testing
    Given I have navigated to '/'
    And I have logged in as a 'user'
    Given I have navigated to '<url>'
    Then I ensure the page is accessible
    Then the page is performant and follows best practices
Examples:
    | url       |
    | filePath  |

@Pending
Scenario Outline: Unauthenticated user - FutureNHS full page testing
    Given I have navigated to '<url>'
    Then I ensure the page is accessible
    Then the page is performant and follows best practices
Examples:
    | url               |
    | alternateFilePath |
