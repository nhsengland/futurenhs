import { GenericPageContent } from '@appTypes/content';
import { User } from '@appTypes/user';

interface Content extends GenericPageContent {}

export interface Props {
    user: User;
    term: string;
    content: Content;
}