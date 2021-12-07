import { GroupsPageContent } from '@appTypes/content';
import { User } from '@appTypes/user';
import { ServicePaginatedResponse } from '@appTypes/service';
import { Group } from '@appTypes/group';

export interface Props {
    user: User;
    content: GroupsPageContent;
    groupsList: ServicePaginatedResponse<Array<Group>>;
}