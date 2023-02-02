import { useRef, useEffect } from 'react'
import classNames from 'classnames'
import FlipMove from 'react-flip-move'

import { simpleClone, deleteArrayItem, moveArrayItem } from '@helpers/util/data'
import { formTypes } from '@constants/forms'
import { cmsBlocks } from '@constants/blocks'
import { useTheme } from '@helpers/hooks/useTheme'
import { useFormConfig } from '@helpers/hooks/useForm'
import { useCsrf } from '@helpers/hooks/useCsrf'
import { Heading } from '@components/layouts/Heading'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { Form } from '@components/old_forms/Form'
import { SVGIcon } from '@components/generic/SVGIcon'
import { Theme } from '@appTypes/theme'
import { FormConfig } from '@appTypes/form'
import { CmsContentBlock } from '@appTypes/contentBlock'

import { Props } from './interfaces'

export const KeyLinksBlock: (props: Props) => JSX.Element = ({
    block,
    headingLevel,
    themeId,
    isEditable,
    isPreview,
    createAction,
    changeAction,
    initialErrors,
    maxLinks = 10,
    className,
}) => {
    const elementIdToFocus = useRef(null)

    const blockId: string = block?.item?.id
    const { title, blocks } = block?.content ?? {}

    const hasLinks: boolean = blocks?.length > 0
    const hasMaxLinks: boolean = blocks?.length === maxLinks

    const { background, content: contentTheme }: Theme = useTheme(themeId)
    const csrfToken: string = useCsrf()
    const formConfig: FormConfig = useFormConfig(
        formTypes.CONTENT_BLOCK_QUICK_LINKS_WRAPPER,
        {
            initialValues: { [`title-${blockId}`]: title },
            errors: initialErrors[blockId] ?? {},
        }
    )

    const generatedClasses: any = {
        wrapper: classNames(className),
        heading: classNames('nhsuk-heading-m', 'u-mb-3'),
        list: classNames('u-list-none u-p-0 u-w-full u-m-0'),
    }

    const handleAddChildBlock = (): void => {
        if (!hasMaxLinks) {
            createAction?.(cmsBlocks.KEY_LINK, blockId).then(
                (createdBlockId: string) => {
                    const updatedBlock: CmsContentBlock = simpleClone(block)
                    const newBlock: CmsContentBlock = {
                        item: {
                            id: createdBlockId,
                            contentType: cmsBlocks.KEY_LINK,
                        },
                        content: {
                            linkText: '',
                            url: '',
                        },
                    }

                    updatedBlock.content.blocks = Array.isArray(
                        updatedBlock.content?.blocks
                    )
                        ? updatedBlock.content.blocks
                        : []
                    updatedBlock.content.blocks.push(newBlock)
                    elementIdToFocus.current = createdBlockId

                    changeAction?.({ block: updatedBlock })
                }
            )
        }
    }

    /**
     * Handle moving a child block
     */
    const handleMoveChildBlock = (blockId: string, offSet: number): void => {
        const updatedBlock: CmsContentBlock = simpleClone(block)
        const childBlocks: Array<CmsContentBlock> =
            updatedBlock.content.blocks ?? []

        const index: number = childBlocks.findIndex(
            (childBlock) => childBlock.item?.id === blockId
        )
        const targetIndex: number = index + offSet
        const updatedChildBlocks: Array<CmsContentBlock> = moveArrayItem(
            childBlocks,
            index,
            targetIndex
        )

        updatedBlock.content.blocks = updatedChildBlocks
        elementIdToFocus.current = blockId

        changeAction?.({ block: updatedBlock })
    }

    const handleChange = (
        formState: Record<string, any>,
        childBlockId?: string
    ): void => {
        const { values, errors, visited } = formState

        const updatedBlock: CmsContentBlock = simpleClone(block)
        const blockIdToUse: string = childBlockId ?? blockId
        const content: Record<string, any> = childBlockId
            ? updatedBlock.content.blocks.find(
                  (block) => block.item.id === childBlockId
              )?.content ?? {}
            : updatedBlock.content

        /**
         * Handle value updates
         */
        if (content) {
            for (const key in content) {
                const fieldName: string = `${key}-${blockIdToUse}`
                const value: any = values[fieldName]

                /**
                 * If a field has been interacted with
                 */
                if (visited[fieldName] && content[key] !== value) {
                    content[key] = value ?? null
                }
            }
        }

        changeAction?.({ block: updatedBlock, errors, childBlockId })
    }

    useEffect(() => {
        if (elementIdToFocus.current) {
            document.getElementById(blockId).focus()
            elementIdToFocus.current = null
        }
    }, [block])

    if (isEditable) {
        return (
            <LayoutColumnContainer>
                <LayoutColumn desktop={9}>
                    <Form
                        key={blockId}
                        csrfToken={csrfToken}
                        instanceId={blockId}
                        formConfig={formConfig}
                        shouldRenderSubmitButton={false}
                        changeAction={handleChange}
                    />
                </LayoutColumn>
                <LayoutColumn>
                    <ul className={generatedClasses.list}>
                        <FlipMove
                            typeName={null}
                            enterAnimation="fade"
                            leaveAnimation="fade"
                            duration={100}
                        >
                            {block.content.blocks?.map((childBlock, index) => {
                                const { item, content } = childBlock

                                const childBlockId: string = item.id
                                const key: string = `${childBlockId}-${index}`

                                const formConfig: FormConfig = useFormConfig(
                                    formTypes.CONTENT_BLOCK_QUICK_LINK,
                                    {
                                        initialValues: {
                                            [`linkText-${childBlockId}`]:
                                                content?.linkText,
                                            [`url-${childBlockId}`]:
                                                content?.url,
                                        },
                                        errors:
                                            initialErrors[childBlockId] ?? {},
                                    }
                                )

                                const shouldRenderMovePrevious: boolean =
                                    index > 0
                                const shouldRenderMoveNext: boolean =
                                    index < block.content.blocks.length - 1
                                const handleChildChange = (
                                    formState: Record<any, any>
                                ): void => handleChange(formState, childBlockId)
                                const handleChildDelete = (
                                    event: any
                                ): void => {
                                    event.preventDefault()

                                    const updatedBlock: CmsContentBlock =
                                        simpleClone(block)
                                    const childBlocks: Array<CmsContentBlock> =
                                        updatedBlock.content.blocks ?? []
                                    const childBlockIndex: number =
                                        childBlocks.findIndex(
                                            (childBlock) =>
                                                childBlock.item.id ===
                                                childBlockId
                                        )

                                    if (childBlockIndex > -1) {
                                        updatedBlock.content.blocks =
                                            deleteArrayItem(
                                                childBlocks,
                                                childBlockIndex
                                            )

                                        if (initialErrors[childBlockId]) {
                                            delete initialErrors[childBlockId]
                                        }

                                        changeAction?.({
                                            block: updatedBlock,
                                            errors: {},
                                        })
                                    }
                                }

                                const handleChildMovePrevious = (
                                    event: any
                                ): void => {
                                    event.preventDefault()

                                    handleMoveChildBlock(childBlockId, -1)
                                }

                                const handleChildMoveNext = (
                                    event: any
                                ): void => {
                                    event.preventDefault()

                                    handleMoveChildBlock(childBlockId, +1)
                                }

                                const generatedClasses: any = {
                                    itemWrapper: classNames(
                                        'u-mb-0',
                                        'focus:u-outline-none'
                                    ),
                                    orderingWrapper: classNames(
                                        'u-flex u-flex-col u-justify-center u-w-4 u-mr-5',
                                        {
                                            ['u-justify-center']:
                                                !shouldRenderMovePrevious ||
                                                !shouldRenderMoveNext,
                                            ['u-justify-between']:
                                                shouldRenderMovePrevious &&
                                                shouldRenderMoveNext,
                                        }
                                    ),
                                    orderingButton: classNames(
                                        'o-link-button u-w-4 u-h-3'
                                    ),
                                }

                                return (
                                    <li
                                        key={key}
                                        id={childBlockId}
                                        tabIndex={-1}
                                        className={generatedClasses.itemWrapper}
                                    >
                                        <LayoutColumnContainer className="u-items-end">
                                            <LayoutColumn desktop={10}>
                                                <Form
                                                    key={key}
                                                    csrfToken={csrfToken}
                                                    instanceId={childBlockId}
                                                    formConfig={formConfig}
                                                    shouldRenderSubmitButton={
                                                        false
                                                    }
                                                    changeAction={
                                                        handleChildChange
                                                    }
                                                    bodyClassName="tablet:u-flex tablet:u-items-end"
                                                />
                                            </LayoutColumn>
                                            <LayoutColumn
                                                desktop={2}
                                                className="u-flex u-mb-6 u-h-10 u-fill-theme-8"
                                            >
                                                <div
                                                    className={
                                                        generatedClasses.orderingWrapper
                                                    }
                                                >
                                                    {shouldRenderMovePrevious && (
                                                        <button
                                                            type="button"
                                                            className={
                                                                generatedClasses.orderingButton
                                                            }
                                                            aria-label="Move link up"
                                                            onClick={
                                                                handleChildMovePrevious
                                                            }
                                                        >
                                                            <SVGIcon name="icon-chevron-up" />
                                                        </button>
                                                    )}
                                                    {shouldRenderMoveNext && (
                                                        <button
                                                            type="button"
                                                            className={
                                                                generatedClasses.orderingButton
                                                            }
                                                            aria-label="Move link down"
                                                            onClick={
                                                                handleChildMoveNext
                                                            }
                                                        >
                                                            <SVGIcon name="icon-chevron-down" />
                                                        </button>
                                                    )}
                                                </div>
                                                <button
                                                    type="button"
                                                    className="o-link-button u-text-size-standard"
                                                    onClick={handleChildDelete}
                                                >
                                                    <SVGIcon
                                                        name="icon-delete"
                                                        className="u-w-5 u-h-5 u-mr-2 u-align-middle"
                                                    />
                                                    <span className="u-align-middle">
                                                        Delete link
                                                    </span>
                                                </button>
                                            </LayoutColumn>
                                        </LayoutColumnContainer>
                                    </li>
                                )
                            })}
                        </FlipMove>
                    </ul>
                </LayoutColumn>
                <LayoutColumn>
                    <button
                        disabled={hasMaxLinks}
                        onClick={handleAddChildBlock}
                        className="c-button c-button--secondary"
                    >
                        Add a link
                    </button>
                </LayoutColumn>
            </LayoutColumnContainer>
        )
    }

    return (
        <div id={blockId} className={generatedClasses.wrapper}>
            <Heading
                headingLevel={headingLevel}
                className={generatedClasses.heading}
            >
                {title}
            </Heading>
            {hasLinks && (
                <LayoutColumnContainer>
                    <ul className={generatedClasses.list}>
                        {blocks?.map(({ content }, index) => {
                            if (content) {
                                const generatedClasses: any = {
                                    itemWrapper: classNames('u-mb-0'),
                                    link: classNames(
                                        'u-block c-button c-button--themeable u-w-full u-drop-shadow u-mb-3',
                                        `u-bg-theme-${background}`,
                                        `u-text-theme-${contentTheme}`
                                    ),
                                }

                                return (
                                    <li
                                        key={index}
                                        className={generatedClasses.itemWrapper}
                                    >
                                        <LayoutColumn tablet={6} desktop={4}>
                                            {isPreview ? (
                                                <span
                                                    className={
                                                        generatedClasses.link
                                                    }
                                                >
                                                    {content.linkText}
                                                </span>
                                            ) : (
                                                <a
                                                    href={content.url}
                                                    className={
                                                        generatedClasses.link
                                                    }
                                                    rel="noreferrer"
                                                >
                                                    {content.linkText}
                                                </a>
                                            )}
                                        </LayoutColumn>
                                    </li>
                                )
                            }

                            return null
                        })}
                    </ul>
                </LayoutColumnContainer>
            )}
        </div>
    )
}
