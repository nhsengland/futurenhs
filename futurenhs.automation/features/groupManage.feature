@Pending
#Edit group information option is no longer available in the app. vNext and Page Manager will overwrite these scripts meaning refactoring is required.
Feature: groupManage
    User journeys covering Administrator functionality on managing a group

Background:
    Given I have navigated to '/'
    And I have logged in as a 'admin'
    When I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed
 

Scenario: FNHS31 - Edit Group Information Page Validation
    When I click the 'Automation Editable Group' link
    Then the 'Automation Editable Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group' header is displayed
    And the 'Group name' field contains 'Automation Editable Group'
    And the 'Strap line' text area contains 'DO NOT USE - This group is reserved solely for use by our automated test scripts'
    And the 'Logo' label is displayed
    And the 'Group introduction' text area contains 'Introduction '
    And the 'Group rules' label is displayed
    

Scenario Outline: FNHS32 - Make a Group Public/Private
    When I click the 'Automation Editable Group' link
    Then the 'Automation Editable Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group' header is displayed
    When I select the 'Is the group public?' checkbox
    And I click the 'Save and close' option
    Given I log off and log in as a 'user'
    Then the 'Automation Editable Group' header is displayed
    And the 'Join Group' link is displayed
    And I click the 'Forum' tab
    Then the 'All Discussions' header is displayed
    And the '<accesstext>' textual value is displayed
Examples:
    | accesstext                                                |
    | Currently no discussions in this Group                    |
    | You must be a member of this group to access this content |


Scenario: FNHS33 - Edit Group Information
    When I click the 'Automation Editable Group' link
    Then the 'Automation Editable Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group' header is displayed
    When I enter 'Automation Edited Group' into the 'Group name' field
    And I enter 'Strapline' into the 'Strap line' text area
    And I enter 'An edited introduction' into the 'Group introduction' text area
    And I enter 'Some stuff' into the 'Group rules' text area
    And I click the 'Save and close' option
    Then the 'Automation Edited Group' header is displayed
    And the 'Strapline' textual value is displayed
    And the 'An edited introduction' textual value is displayed
    And I click the 'About Us' tab
    Then the 'About Us' header is displayed
    And the 'Some stuff' textual value is displayed


Scenario Outline: FNHS34 - Edit Group Information Field Boundary Validation
    When I click the 'Automation Edited Group' link
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group' header is displayed
    When I enter '<name>' into the 'Group name' field
    And I enter '<strapline>' into the 'Strap line' text area
    And I enter '<introduction>' into the 'Group introduction' text area
    And I enter '<rules>' into the 'Group rules' text area
    And I click the 'Save and close' option
    Then the 'There is a problem' header is displayed
    Then the '<error message>' error summary is displayed
    And the '<error message>' textual value is displayed
Examples:
    | name                    | strapline | introduction | rules | error message                                |
    |                         | Strapline | introduction | rules | Please provide a name for your group         |
    | Automation Edited Group | Strapline |              | rules | Please provide an introduction to your group |
    | 0GHIInVfZqvL7xl4HNNQnecIuFSzniTxetbyG9NZWZUfyj4TAMCxY5y0gmUFSDY7SMqYeDQCvrfL0pfEAzuA8FOgTNpRXv2QbIkZ58ZqiugWYIoD7Vbrslz5htK048ulLszKJuYXoEPYgUTCR4i7wFm86LsZ2UJ5gndleVGARbBTXmSTxLCdA5Mji4Lygj5OgQ3skNQcxcRujZsRfhgfN6pFaOr3BddLQv75FvFZzKXDg4qiKOymmSqL4LwYbCoi | Strapline | Introduction | Rules | The group name must not be greater than 255 characters. |
    | Automation Edited Group | 34UdKAHF9iJsgQkxcFjzwrNPWjE0wuYxGclZHbawhTX22LnVGViQdxHeRc37YBhiomZdYmY7uhPWEAm3tea9ADYGNlHcAuCqjAuxR8yJe4KzTO3o641WqmZ1px1n4ND7WhZQWVt8wsiIKea3mmnleTyNR2gV9wk7NP3KbkSJhYJJ1j35cqI5VmQyEzX12WXjXKiNwiUVFRlZBpS8stN9up2RwRwytdqrmTLFDF3DMbWYhN9nIuZvZ4pkfnBotT0PD4TbkLHcZHS6z0CIoWulGQJo9ZmRLjaaFSRMNcDdslG0ndAI80caTpxABkPtLBjJfvZShlEHQmm2IrnaCTkZTu5i44Ntf9DwWkOIH1sRVkxfIZ1vGTn0TzcoxPyewuwxEgIhXQp7tsy3VxbugHSBfS5oBST2sEyLJFLAZgwkQoqozAjGhMrPNzT775jRlX1AVKNBPQKCq0yrRbTPesy5tJA1W08zDQjF8wCY4dOdvpuDTeJW3wWzIuuG5Sqv53HOabXm6S5BXQPjhZmtGNJLqYU2pbCqgxjdweZSPIzysTwItka6d3dCaFa0hzcT8fjrdWIU5h4vu26Tl7OUXD438vSgjyylsLstmBhILs78PhkoLs9322ks53hhVN6mzPJhMTF3aQDHFhzT3xXj6Q1QTSGKEvw5LfC9Mbg3C4TOYKVNFeBSQ7GtMOd4kHHkAPsRP9kaR7aj9rRlBO2AzkmINxhIRicEInhnX2B1OrXFBjskEoT8EFWluNbtAmfwBFXMi8Bey4eaFWxrY0GTXLhWsHocuOI2tKp7dEIvzYwpnbfc9m2Q2ko5NSd6EzM3waknTpEMHtV0ADjECE10jkIRzJsSoutujMEvlCowKYkZgQM7zZyHDzVn4u9KYY7jojaxZ9kt67H25vUvv44Hjpx5CHLPt9LGp9pY45Tnq42cYlP9ibfKSTjaRIJShNEY9BaT0KTJlkBj2aaG15OxoemVoL2w8oPensZyzyLd8htMz | Introduction | Rules | The strap line must not be greater than 1000 characters. |
    | Automation Edited Group | Strapline | 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 | rules | The group introduction must not be greater than 4000 characters. |
    | Automation Edited Group | Strapline | introduction | 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 | The group rules must not be greater than 4000 characters. |


Scenario Outline: FNHS35 - Edit Group Information Image Upload
    When I click the 'Automation Edited Group' link
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group' header is displayed
    When I upload the '<image>' file
    And I click the 'Save and close' option
    Then the 'Automation Edited Group' header is displayed
    And the group image is displayed
Examples:
    | image                 |
    | /media/test.png       |
    | /media/test.jpg       |
    | /media/largeimage.png |


Scenario Outline: FNHS36 - Edit Group Information Image Upload Error Validation
    When I click the 'Automation Edited Group' link
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group' header is displayed    
    When I upload the '<image>' file
    And I click the 'Save and close' option
    Then the '<error message>' error summary is displayed
    And the '<error message>' textual value is displayed
Examples:
    | image                    | error message                                                  |    
    | /media/test.gif          | The selected file must be a JPG or PNG                         |
    | /media/toolargeimage.png | The logo file is too large. It must not be greater then 500KB  |