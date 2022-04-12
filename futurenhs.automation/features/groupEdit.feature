Feature: groupEdit
    User journeys covering Edit Group Information form functionality as a group admin

Background:
    Given I have navigated to '/'
    And I have logged in as a 'group admin' and accept the cookies
    Then the 'My Groups' header is displayed
 

Scenario: FNHS31 - Edit Group Information Page Validation
    When I select the 'Automation Editable Group' group card
    Then the 'Automation Editable Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    And the 'Group name' field contains 'Automation Editable Group'
    And the 'Strap line' text area contains 'DO NOT USE - This group is reserved solely for use by our automated test scripts'
    And the 'Logo (optional)' label is displayed
    And the 'Save and close' button is displayed
    And the 'Discard changes' link is displayed


Scenario: FNHS33 - Edit Group Information
    When I select the 'Automation Editable Group' group card
    Then the 'Automation Editable Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    When I enter 'Automation Edited Group' into the 'Group name' field
    And I enter 'Strapline' into the 'Strap line' text area
    And I click the 'Save and close' button
    Then the 'Automation Edited Group' header is displayed
    And the 'Strapline' textual value is displayed


Scenario Outline: FNHS34 - Edit Group Information error validation
    When I select the 'Automation Edited Group' group card
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    When I enter '<name>' into the 'Group name' field
    And I enter '<strapline>' into the 'Strap line' text area
    And I click the 'Save and close' button
    Then the 'There is a problem' textual value is displayed
    Then the '<error message>' error summary is displayed
    And the '<error message>' textual value is displayed
Examples:
    | name                    | strapline | error message                                          |
    |                         | Strapline | Enter the group name                                   | 
    | 0GHIInVfZqvL7xl4HNNQnecIuFSzniTxetbyG9NZWZUfyj4TAMCxY5y0gmUFSDY7SMqYeDQCvrfL0pfEAzuA8FOgTNpRXv2QbIkZ58ZqiugWYIoD7Vbrslz5htK048ulLszKJuYXoEPYgUTCR4i7wFm86LsZ2UJ5gndleVGARbBTXmSTxLCdA5Mji4Lygj5OgQ3skNQcxcRujZsRfhgfN6pFaOr3BddLQv75FvFZzKXDg4qiKOymmSqL4LwYbCoi | Strapline | Enter 255 or fewer characters |
    | Automation Edited Group | 34UdKAHF9iJsgQkxcFjzwrNPWjE0wuYxGclZHbawhTX22LnVGViQdxHeRc37YBhiomZdYmY7uhPWEAm3tea9ADYGNlHcAuCqjAuxR8yJe4KzTO3o641WqmZ1px1n4ND7WhZQWVt8wsiIKea3mmnleTyNR2gV9wk7NP3KbkSJhYJJ1j35cqI5VmQyEzX12WXjXKiNwiUVFRlZBpS8stN9up2RwRwytdqrmTLFDF3DMbWYhN9nIuZvZ4pkfnBotT0PD4TbkLHcZHS6z0CIoWulGQJo9ZmRLjaaFSRMNcDdslG0ndAI80caTpxABkPtLBjJfvZShlEHQmm2IrnaCTkZTu5i44Ntf9DwWkOIH1sRVkxfIZ1vGTn0TzcoxPyewuwxEgIhXQp7tsy3VxbugHSBfS5oBST2sEyLJFLAZgwkQoqozAjGhMrPNzT775jRlX1AVKNBPQKCq0yrRbTPesy5tJA1W08zDQjF8wCY4dOdvpuDTeJW3wWzIuuG5Sqv53HOabXm6S5BXQPjhZmtGNJLqYU2pbCqgxjdweZSPIzysTwItka6d3dCaFa0hzcT8fjrdWIU5h4vu26Tl7OUXD438vSgjyylsLstmBhILs78PhkoLs9322ks53hhVN6mzPJhMTF3aQDHFhzT3xXj6Q1QTSGKEvw5LfC9Mbg3C4TOYKVNFeBSQ7GtMOd4kHHkAPsRP9kaR7aj9rRlBO2AzkmINxhIRicEInhnX2B1OrXFBjskEoT8EFWluNbtAmfwBFXMi8Bey4eaFWxrY0GTXLhWsHocuOI2tKp7dEIvzYwpnbfc9m2Q2ko5NSd6EzM3waknTpEMHtV0ADjECE10jkIRzJsSoutujMEvlCowKYkZgQM7zZyHDzVn4u9KYY7jojaxZ9kt67H25vUvv44Hjpx5CHLPt9LGp9pY45Tnq42cYlP9ibfKSTjaRIJShNEY9BaT0KTJlkBj2aaG15OxoemVoL2w8oPensZyzyLd8htMz | Enter 1000 or fewer characters |


Scenario Outline: FNHS35 - Edit group information change logo
    When I select the 'Automation Edited Group' group card
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    When I upload the '<image>' file
    Then the image file '<image>' is uploaded and ready
    When I click the 'Save and close' button
    Then the 'Automation Edited Group' header is displayed
Examples:
    | image                 |
    | /media/test.png       |
    | /media/test.jpg       |
    | /media/largeimage.png |


Scenario Outline: FNHS36 - Edit group information change logo error validation
    When I select the 'Automation Edited Group' group card
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed    
    When I upload the '<image>' file
    And I click the 'Save and close' button
    Then the '<error message>' error summary is displayed
    And the '<error message>' textual value is displayed
Examples:
    | image                    | error message                                 |    
    | /media/test.gif          | The selected file must be a JPG or PNG        |
    | /media/toolargeimage.png | The selected file must be smaller than 500KB  |