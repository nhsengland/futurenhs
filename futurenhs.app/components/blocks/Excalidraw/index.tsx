import React, { useEffect, useState, useRef } from 'react'
import {
    Excalidraw,
    exportToCanvas,
    exportToSvg,
    exportToBlob,
} from '@excalidraw/excalidraw'

import type { ExcalidrawElement } from '@excalidraw/excalidraw/types/element/types'
import {
    AppState,
    ExcalidrawImperativeAPI,
    ExcalidrawProps,
} from '@excalidraw/excalidraw/types/types'

export default function ExcalidrawApp() {
    const excalidrawRef = useRef<ExcalidrawImperativeAPI>(null)

    const [viewModeEnabled, setViewModeEnabled] = useState(false)
    const [zenModeEnabled, setZenModeEnabled] = useState(false)
    const [gridModeEnabled, setGridModeEnabled] = useState(false)
    const [blobUrl, setBlobUrl] = useState<string | null>(null)
    const [canvasUrl, setCanvasUrl] = useState<string | null>(null)
    const [exportWithDarkMode, setExportWithDarkMode] = useState<boolean>(false)
    const [shouldAddWatermark, setShouldAddWatermark] = useState<boolean>(false)
    const [theme, setTheme] = useState<ExcalidrawProps['theme']>('light')

    useEffect(() => {
        const onHashChange = () => {
            const hash = new URLSearchParams(window.location.hash.slice(1))
        }
        window.addEventListener('hashchange', onHashChange, false)
        return () => {
            window.removeEventListener('hashchange', onHashChange)
        }
    }, [])

    const updateScene = () => {
        const sceneData = {
            elements: [
                {
                    type: 'rectangle',
                    version: 141,
                    versionNonce: 361174001,
                    isDeleted: false,
                    id: 'oDVXy8D6rom3H1-LLH2-f',
                    fillStyle: 'hachure',
                    strokeWidth: 1,
                    strokeStyle: 'solid',
                    roughness: 1,
                    opacity: 100,
                    angle: 0,
                    x: 100.50390625,
                    y: 93.67578125,
                    strokeColor: '#c92a2a',
                    backgroundColor: 'transparent',
                    width: 186.47265625,
                    height: 141.9765625,
                    seed: 1968410350,
                    groupIds: [],
                },
            ],
            appState: {
                viewBackgroundColor: '#edf2ff',
            },
        }
    }

    return (
        <div id="Excalidraw">
            <h1> Excalidraw Example</h1>
            <div className="excalidraw-wrapper">
                <Excalidraw
                    ref={excalidrawRef}
                    onChange={(
                        elements: readonly ExcalidrawElement[],
                        state: AppState
                    ) => console.log('Elements :', elements, 'State : ', state)}
                    onPointerUpdate={(payload) => console.log(payload)}
                    viewModeEnabled={viewModeEnabled}
                    zenModeEnabled={zenModeEnabled}
                    gridModeEnabled={gridModeEnabled}
                    theme={theme}
                    name="Custom name of drawing"
                />
            </div>
        </div>
    )
}
