# FutureNHS Sharing Platform 
This repository contains artefacts pertinent to the development of the new FutureNHS sharing platform

## What is FutureNHS?
The FutureNHS Collaboration platform is the only virtual collaboration platform from the NHS that supports people working in health and social care to make change, improve and transform across organisations, places and professions.

To learn more about this project, please visit https://future.nhs.uk/

## Copyright and License

Unless otherwise specified, all content in this repository and any copies are subject to [Crown Copyright](http://www.nationalarchives.gov.uk/information-management/re-using-public-sector-information/copyright-and-re-use/crown-copyright/) under the [Open Government License v3](./OPEN-GOVERNMENT-LICENSE).

Any code is dual licensed under the [MIT license](./LICENSE) and the [Open Government License v3](./OPEN-GOVERNMENT-LICENSE). 

Any new work added to this repository must conform to the conditions of these licenses. In particular this means that this project may not depend on GPL-licensed or AGPL-licensed libraries, as these would violate the terms of those libraries' licenses.

## Pushing work to the FutureNHS repository

Pushing new changes to the Future NHS repository will be a semi-automated process, work will be rebased from SPRINT into MAIN using a pull request, once completed this will trigger the changes to be automatically pushed up to a branch in the FutureNHS Github repository to the futurenhs-sprint branch. This branch will act as an intermediary between the main FutureNHS branch which may contain contributions from others and the work from futurenhs-sprint. 

Once the code is pushed to futurenhs-sprint a pull request will be raised to be approved, any merge conflicts will be dealt with in the futurenhs-sprint branch manually to ensure there are no breaking changes or conflicting changes, once resolved the pull request will be completed and the code will be rebased into the main branch completing the process.

If there are updates from other contributors in the main branch these will be rebased into futurenhs-sprint branch through a pull request, these updates will then be rebased into the SPRINT by the FutureNHS sprint team so they are now part of the sprint teams code base, this will be a manual process as these changes need to be reviewed to ensure there are no breaking changes or conflicting changes
