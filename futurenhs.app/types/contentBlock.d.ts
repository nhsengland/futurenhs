export interface CmsContentBlock {
    instanceId?: string;
    item: {
        id: string;
        name: string;
        contentType: string;
    };
    content: Record<string, any>;
}
