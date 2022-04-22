export interface CmsContentBlock {
    item: {
        id: string;
        name: string;
        contentType: string;
    };
    content: Record<string, any>;
}
