import { GenericPageContent } from '@appTypes/content';

interface Content extends GenericPageContent {}

export interface Props {
    isAuthenticated: boolean;
    content: Content;
}