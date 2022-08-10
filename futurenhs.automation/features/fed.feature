Feature: FED Accessibility and Performance
    Blurb about the feature and test coverage


Scenario Outline: FNHS:FED01 - Lighthouse Performance Test
    Given I have navigated to '/'
    And I have logged in as a 'admin'
    When I have navigated to '<url>'
    Then the page is performant and follows best practices
Examples:
    | url |
    | / |
    | groups/ |
    | groups/aa/ |
    | groups/aa/about |
    | groups/aa/members |
    | groups/aa/members/d74ed860-9ea5-4c95-9394-ad3a00924fa5 |
    | groups/aa/folders |
    | groups/aa/folders/create |
    | groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb |
    | groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb/update |
    | groups/aa/files/54249d0f-3544-4b76-b6fb-ae0001022c09 |
    | groups/aa/files/54249d0f-3544-4b76-b6fb-ae0001022c09/detail |
    | groups/aa/forum |
    | groups/aa/forum/create |
    | groups/aa/forum/3fbc0d4e-c2a7-46a0-99e9-ad7f010eb4ad |
    | groups/aa/members |
    | groups/aa/members/16d4d237-4686-4b4f-ae05-1cff3f8fb43a |
    | groups/aa/members/16d4d237-4686-4b4f-ae05-1cff3f8fb43a/update |
    | groups/aa/update |
    | groups/discover |

    | contact-us/ |
    | cookies/ |
    | privacy-policy/ |
    | terms-and-conditions/ |

    | users/d74ed860-9ea5-4c95-9394-ad3a00924fa5 |
    | users/d74ed860-9ea5-4c95-9394-ad3a00924fa5/update |

    | admin/ |
    | admin/groups |
    | admin/groups/create |
    | admin/users |
    | admin/users/invite |



Scenario Outline: FNHS:FED02 - Axe Accessibility Test
    Given I have navigated to '/'
    And I have logged in as a 'admin'
    Given I have navigated to '<url>'
    Then I ensure the page is accessible
Examples:
    | url |
    | / |
    | groups/ |
    | groups/aa/ |
    | groups/aa/about |
    | groups/aa/members |
    | groups/aa/members/d74ed860-9ea5-4c95-9394-ad3a00924fa5 |
    | groups/aa/folders |
    | groups/aa/folders/create |
    | groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb |
    | groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb/update |
    | groups/aa/files/54249d0f-3544-4b76-b6fb-ae0001022c09 |
    | groups/aa/files/54249d0f-3544-4b76-b6fb-ae0001022c09/detail |
    | groups/aa/forum |
    | groups/aa/forum/create |
    | groups/aa/forum/3fbc0d4e-c2a7-46a0-99e9-ad7f010eb4ad |
    | groups/aa/members |
    | groups/aa/members/16d4d237-4686-4b4f-ae05-1cff3f8fb43a |
    | groups/aa/members/16d4d237-4686-4b4f-ae05-1cff3f8fb43a/update |
    | groups/aa/update |
    | groups/discover |

    | contact-us/ |
    | cookies/ |
    | privacy-policy/ |
    | terms-and-conditions/ |

    | users/d74ed860-9ea5-4c95-9394-ad3a00924fa5 |
    | users/d74ed860-9ea5-4c95-9394-ad3a00924fa5/update |

    | admin/ |
    | admin/groups |
    | admin/groups/create |
    | admin/users |
    | admin/users/invite |
