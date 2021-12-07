import { GenericPageContent } from '@appTypes/content';
import { User } from '@appTypes/user';
import { ServicePaginatedResponse } from '@appTypes/service';
import { Group } from '@appTypes/group';


interface Content extends GenericPageContent {}

export interface Props {
    user: User;
    content: Content;
    groupsList: ServicePaginatedResponse<Array<Group>>;
}