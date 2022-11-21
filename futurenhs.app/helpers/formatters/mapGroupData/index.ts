import { Group } from '@appTypes/group'

export const mapGroupData = (apiData) => {
    const group: Group = apiData
        ? {
              text: {
                  mainHeading: apiData.nameText ?? null,
                  strapLine: apiData.straplineText ?? null,
              } as any,
              groupId: apiData.slug,
              invite: apiData.invite,
              themeId: apiData.themeId,
              totalMemberCount: apiData.memberCount ?? 0,
              totalDiscussionCount: apiData.discussionCount ?? 0,
              totalFileCount: apiData.fileCount ?? 0,
              isPublic: apiData.isPublic,
              image: apiData.image
                  ? {
                        src: `${apiData.image?.source}`,
                        height: apiData.image?.height ?? null,
                        width: apiData.image?.width ?? null,
                        altText: `Group image for ${apiData.nameText}`,
                    }
                  : null,
          }
        : null
    return group
}
